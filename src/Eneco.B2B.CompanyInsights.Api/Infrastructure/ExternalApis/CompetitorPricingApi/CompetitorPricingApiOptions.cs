namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi
{
    public sealed record CompetitorPricingApiOptions : IExternalApiOptions
    {
        public const string SectionName = "CompetitorPricingApi";

        public required string BaseUrl { get; init; } = string.Empty;
        public required string ApiKey { get; init; } = string.Empty;
        public required int TotalRequestTimeoutInSeconds { get; init; } = 30;
    }
}
