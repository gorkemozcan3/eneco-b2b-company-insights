using Eneco.B2B.CompanyInsights.Api.Exceptions;
using Eneco.B2B.CompanyInsights.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eneco.B2B.CompanyInsights.Tests.Unit
{
    public class GlobalExceptionHandlerTests
    {
        private readonly IProblemDetailsService _problemDetailsService = Substitute.For<IProblemDetailsService>();
        private readonly GlobalExceptionHandler _sut;

        public GlobalExceptionHandlerTests()
        {
            _problemDetailsService
                .TryWriteAsync(Arg.Any<ProblemDetailsContext>())
                .Returns(true);

            _sut = new GlobalExceptionHandler(_problemDetailsService, NullLogger<GlobalExceptionHandler>.Instance);
        }

        public static TheoryData<Exception, int> ExceptionsWithExpectedStatusCode() => new()
        {
            { new HttpRequestException("not found", null, HttpStatusCode.NotFound), StatusCodes.Status404NotFound },
            { new ExternalApiException("unusable body"), StatusCodes.Status502BadGateway },
            { new HttpRequestException("upstream down"), StatusCodes.Status502BadGateway },
            { new TaskCanceledException("timed out"), StatusCodes.Status504GatewayTimeout },
            { new InvalidOperationException("a bug"), StatusCodes.Status500InternalServerError }
        };

        [Theory]
        [MemberData(nameof(ExceptionsWithExpectedStatusCode))]
        public async Task TryHandleAsync_SetsExpectedStatusCode(Exception exception, int expectedStatusCode)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            // Act
            await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.Equal(expectedStatusCode, httpContext.Response.StatusCode);
        }

        public static TheoryData<Exception, int, string> ExceptionsWithExpectedProblemDetails() => new()
        {
            {
                new HttpRequestException("not found", null, HttpStatusCode.NotFound),
                StatusCodes.Status404NotFound,
                "The requested resource was not found."
            },
            {
                new ExternalApiException("unusable body"),
                StatusCodes.Status502BadGateway,
                "An external API returned an unusable response."
            },
            {
                new InvalidOperationException("a bug"),
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred."
            }
        };

        [Theory]
        [MemberData(nameof(ExceptionsWithExpectedProblemDetails))]
        public async Task TryHandleAsync_HandledException_WritesProblemDetailsWithMatchingStatusAndTitle(
            Exception exception,
            int expectedStatusCode,
            string expectedTitle)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            ProblemDetailsContext? writtenContext = null;

            _problemDetailsService
                .TryWriteAsync(Arg.Do<ProblemDetailsContext>(context => writtenContext = context))
                .Returns(true);

            // Act
            await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.NotNull(writtenContext);
            Assert.Equal(expectedStatusCode, writtenContext!.ProblemDetails.Status);
            Assert.Equal(expectedTitle, writtenContext.ProblemDetails.Title);
            Assert.Same(exception, writtenContext.Exception);
        }

        public static TheoryData<Exception> ExceptionsWithSensitiveMessage() =>
        [
            new HttpRequestException("secret-host:5432 refused the connection", null, HttpStatusCode.NotFound),
            new ExternalApiException("The response from 'https://internal.kvk/api?key=s3cr3t' was invalid."),
            new InvalidOperationException("Connection string 'Server=prod;Password=hunter2' is invalid.")
        ];

        [Theory]
        [MemberData(nameof(ExceptionsWithSensitiveMessage))]
        public async Task TryHandleAsync_HandledException_DoesNotLeakExceptionMessageInProblemDetails(Exception exception)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            ProblemDetailsContext? writtenContext = null;

            _problemDetailsService
                .TryWriteAsync(Arg.Do<ProblemDetailsContext>(context => writtenContext = context))
                .Returns(true);

            // Act
            await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.NotNull(writtenContext);
            Assert.DoesNotContain(exception.Message, writtenContext!.ProblemDetails.Title);
            Assert.Null(writtenContext.ProblemDetails.Detail);
        }
    }
}
