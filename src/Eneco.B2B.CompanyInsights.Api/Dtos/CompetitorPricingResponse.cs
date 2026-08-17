namespace Eneco.B2B.CompanyInsights.Api.Dtos
{
    public record CompetitorPricingResponse(
        string KvkNumber,
        IReadOnlyList<PriceResponse> Prices);
}
