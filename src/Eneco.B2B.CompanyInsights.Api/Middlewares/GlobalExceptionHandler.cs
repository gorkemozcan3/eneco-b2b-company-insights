using Eneco.B2B.CompanyInsights.Api.Exceptions;
using Eneco.B2B.CompanyInsights.Api.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Polly.Timeout;
using System.Net;

namespace Eneco.B2B.CompanyInsights.Api.Middlewares
{
    public sealed class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = ExceptionMapper(exception);

            _logger.LogUnhandledException(exception, statusCode);

            httpContext.Response.StatusCode = statusCode;

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title
                }
            });
        }

        private static (int StatusCode, string Title) ExceptionMapper(Exception exception) => exception switch
        {
            HttpRequestException { StatusCode: HttpStatusCode.NotFound }
                => (StatusCodes.Status404NotFound, "The requested resource was not found."),

            ExternalApiException
                => (StatusCodes.Status502BadGateway, "An external API returned an unusable response."),

            HttpRequestException
                => (StatusCodes.Status502BadGateway, "An external API could not be reached."),

            TaskCanceledException or TimeoutException or TimeoutRejectedException
                => (StatusCodes.Status504GatewayTimeout, "An external API did not respond in time."),

            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };
    }
}
