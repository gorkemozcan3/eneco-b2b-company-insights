namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis
{
    public interface IExternalApiOptions
    {
        string BaseUrl { get; }
        string ApiKey { get; }
        int TotalRequestTimeoutInSeconds { get; }
    }
}
