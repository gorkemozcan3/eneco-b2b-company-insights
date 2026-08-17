using Eneco.B2B.CompanyInsights.Api.Exceptions;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi.Contracts;
using Eneco.B2B.CompanyInsights.Tests.TestStubs;
using System.Text.Json;

namespace Eneco.B2B.CompanyInsights.Tests.Unit
{
    public class HttpClientJsonExtensionsTests
    {
        private const string RequestUri = "companies/12345679";

        public sealed record TestPayload(string Name);

        [Fact]
        public async Task GetFromJsonOrThrowAsync_SuccessResponse_ReturnsDeserializedResult()
        {
            // Arrange
            var handler = HttpMessageHandlerStub.WithJsonResponse("""
                {
                    "kvkNumber": "12345679",
                    "name": "Eneco B.V.",
                    "address": {
                        "street": "Marten Meesweg",
                        "houseNumber": "5",
                        "postalCode": "3068AV",
                        "city": "Rotterdam"
                    },
                    "industry": "Energy"
                }
                """);

            using var httpClient = handler.CreateClient();

            // Act
            var result = await httpClient.GetFromJsonOrThrowAsync<Company>(RequestUri, TestContext.Current.CancellationToken);

            // Assert
            var expected = new Company(
                KvkNumber: "12345679",
                Name: "Eneco B.V.",
                Address: new CompanyAddress("Marten Meesweg", "5", "3068AV", "Rotterdam"),
                Industry: "Energy");

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetFromJsonOrThrowAsync_NotFoundResponse_ThrowsHttpRequestExceptionWithNotFoundStatusCode()
        {
            // Arrange
            var handler = HttpMessageHandlerStub.WithStatusCode(HttpStatusCode.NotFound);

            using var httpClient = handler.CreateClient();

            // Act
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => httpClient.GetFromJsonOrThrowAsync<Company>(RequestUri, TestContext.Current.CancellationToken));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task GetFromJsonOrThrowAsync_ServerErrorResponse_ThrowsHttpRequestExceptionWithServerErrorStatusCode()
        {
            // Arrange
            var handler = HttpMessageHandlerStub.WithStatusCode(HttpStatusCode.InternalServerError);

            using var httpClient = handler.CreateClient();

            // Act
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => httpClient.GetFromJsonOrThrowAsync<Company>(RequestUri, TestContext.Current.CancellationToken));

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        }

        [Fact]
        public async Task GetFromJsonOrThrowAsync_NullJsonBody_ThrowsExternalApiException()
        {
            // Arrange
            var handler = HttpMessageHandlerStub.WithJsonResponse("null");

            using var httpClient = handler.CreateClient();

            // Act
            var exception = await Assert.ThrowsAsync<ExternalApiException>(
                () => httpClient.GetFromJsonOrThrowAsync<TestPayload>(RequestUri, TestContext.Current.CancellationToken));

            // Assert
            Assert.Contains(RequestUri, exception.Message);
            Assert.Contains(nameof(TestPayload), exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetFromJsonOrThrowAsync_EmptyBody_ThrowsExternalApiException(string body)
        {
            // Arrange
            var handler = HttpMessageHandlerStub.WithJsonResponse(body);

            using var httpClient = handler.CreateClient();

            // Act
            var exception = await Assert.ThrowsAsync<ExternalApiException>(
                () => httpClient.GetFromJsonOrThrowAsync<Company>(RequestUri, TestContext.Current.CancellationToken));

            // Assert
            Assert.Contains(RequestUri, exception.Message);
        }

        [Theory]
        [InlineData("<html>Gateway Timeout</html>")]
        [InlineData("{ wefewf: \"\" ")]
        public async Task GetFromJsonOrThrowAsync_MalformedJsonBody_ThrowsExternalApiExceptionWithJsonInnerException(string body)
        {
            // Arrange
            var handler = HttpMessageHandlerStub.WithJsonResponse(body);

            using var httpClient = handler.CreateClient();

            // Act
            var exception = await Assert.ThrowsAsync<ExternalApiException>(
                () => httpClient.GetFromJsonOrThrowAsync<TestPayload>(RequestUri, TestContext.Current.CancellationToken));

            // Assert
            Assert.Contains(RequestUri, exception.Message);
            Assert.Contains(nameof(TestPayload), exception.Message);
            Assert.IsType<JsonException>(exception.InnerException);
        }

        [Fact]
        public async Task GetFromJsonOrThrowAsync_AnyRequest_SendsGetRequestToRequestUri()
        {
            // Arrange
            var handler = HttpMessageHandlerStub.WithJsonResponse("""{ "name": "Eneco B.V." }""");

            using var httpClient = handler.CreateClient();

            // Act
            await httpClient.GetFromJsonOrThrowAsync<TestPayload>(RequestUri, TestContext.Current.CancellationToken);

            // Assert
            var request = Assert.Single(handler.ReceivedRequests);
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("https://api.example.com/companies/12345679", request.RequestUri!.ToString());
        }

        [Fact]
        public async Task GetFromJsonOrThrowAsync_CancelledToken_ThrowsOperationCanceledExceptionWithoutSendingRequest()
        {
            // Arrange
            var handler = HttpMessageHandlerStub.WithJsonResponse("""{ "name": "Eneco B.V." }""");

            using var httpClient = handler.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource();
            await cancellationTokenSource.CancelAsync();

            // Act
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => httpClient.GetFromJsonOrThrowAsync<TestPayload>(RequestUri, cancellationTokenSource.Token));

            // Assert
            Assert.Empty(handler.ReceivedRequests);
        }
    }
}
