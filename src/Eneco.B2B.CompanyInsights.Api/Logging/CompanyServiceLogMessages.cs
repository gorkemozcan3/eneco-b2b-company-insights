namespace Eneco.B2B.CompanyInsights.Api.Logging
{
    /// <summary>
    /// Source-generated log messages emitted by <see cref="Services.CompanyService"/>.
    /// </summary>
    public static partial class CompanyServiceLogMessages
    {
        [LoggerMessage(
            EventId = 1000,
            Level = LogLevel.Debug,
            Message = "Retrieving company {KvkNumber} from the KvK Finder API.")]
        public static partial void LogRetrievingCompany(this ILogger logger, string kvkNumber);

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Retrieved company {KvkNumber}.")]
        public static partial void LogRetrievedCompany(this ILogger logger, string kvkNumber);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Debug,
            Message = "Retrieving competitor pricing for company {KvkNumber}.")]
        public static partial void LogRetrievingCompetitorPricing(this ILogger logger, string kvkNumber);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "Retrieved competitor pricing for company {KvkNumber}: returning {ReturnedPriceCount} of {TotalPriceCount} price(s) after filtering to electricity products.")]
        public static partial void LogRetrievedCompetitorPricing(this ILogger logger, string kvkNumber, int returnedPriceCount, int totalPriceCount);
    }
}
