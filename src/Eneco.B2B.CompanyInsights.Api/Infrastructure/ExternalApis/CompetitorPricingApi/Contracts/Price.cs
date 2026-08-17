namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi.Contracts
{
    public sealed record Price(
        string Product,
        decimal? PricePerKwh,
        decimal? PricePerM3,
        string Currency);
}
