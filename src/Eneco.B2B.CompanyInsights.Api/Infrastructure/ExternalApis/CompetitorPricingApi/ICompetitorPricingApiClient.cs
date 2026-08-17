using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi.Contracts;

namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi
{
    public interface ICompetitorPricingApiClient
    {
        Task<CompetitorPricingInfo> GetCompetitorPricingAsync(string kvkNumber, CancellationToken cancellationToken);
    }
}
