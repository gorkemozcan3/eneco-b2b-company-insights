namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi.Contracts
{
    public sealed record CompetitorPricingInfo(
        string CompanyId,
        IReadOnlyList<Price> Prices);
}
