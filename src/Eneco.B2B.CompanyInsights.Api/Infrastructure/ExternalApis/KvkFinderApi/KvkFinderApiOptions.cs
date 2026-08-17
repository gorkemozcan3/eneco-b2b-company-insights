namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi
{
    public sealed record KvkFinderApiOptions : IExternalApiOptions
    {
        public const string SectionName = "KvkFinderApi";

        public required string BaseUrl { get; init; } = string.Empty;
        public required string ApiKey { get; init; } = string.Empty;
        public required int TotalRequestTimeoutInSeconds { get; init; } = 30;
    }
}
