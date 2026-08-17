namespace Eneco.B2B.CompanyInsights.Api.Logging
{
    /// <summary>
    /// Source-generated log messages emitted by <see cref="Middlewares.GlobalExceptionHandler"/>.
    /// </summary>
    public static partial class GlobalExceptionHandlerLogMessages
    {
        [LoggerMessage(
            EventId = 2000,
            Level = LogLevel.Error,
            Message = "Unhandled exception. Responding with {StatusCode}.")]
        public static partial void LogUnhandledException(this ILogger logger, Exception exception, int statusCode);
    }
}
