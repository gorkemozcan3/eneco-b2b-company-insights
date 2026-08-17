using Eneco.B2B.CompanyInsights.Api.Dtos;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi;
using Eneco.B2B.CompanyInsights.Api.Logging;
using Eneco.B2B.CompanyInsights.Api.Services.Interfaces;

namespace Eneco.B2B.CompanyInsights.Api.Services
{
    public sealed class CompanyService(
        IKvkFinderApiClient kvkFinderApiClient,
        ICompetitorPricingApiClient competitorPricingApiClient,
        ILogger<CompanyService> logger) : ICompanyService
    {
        private readonly IKvkFinderApiClient _kvkFinderApiClient = kvkFinderApiClient;
        private readonly ICompetitorPricingApiClient _competitorPricingApiClient = competitorPricingApiClient;
        private readonly ILogger<CompanyService> _logger = logger;

        public async Task<CompanyResponse> GetCompanyAsync(string kvkNumber, CancellationToken cancellationToken)
        {
            _logger.LogRetrievingCompany(kvkNumber);

            var company = await _kvkFinderApiClient.GetCompanyAsync(kvkNumber, cancellationToken);

            _logger.LogRetrievedCompany(kvkNumber);

            return new CompanyResponse(
                KvkNumber: company.KvkNumber,
                CompanyName: company.Name,
                PostalCode: company.Address.PostalCode,
                City: company.Address.City,
                Industry: company.Industry);
        }

        public async Task<CompetitorPricingResponse> GetCompetitorPricingAsync(string kvkNumber, CancellationToken cancellationToken)
        {
            _logger.LogRetrievingCompetitorPricing(kvkNumber);

            var competitorPricing = await _competitorPricingApiClient.GetCompetitorPricingAsync(kvkNumber, cancellationToken);

            // Assuming with the PricePerKwh property, it indicates the competitors are electricity providers, and we only want to return those
            // requires clarification from the product owner, but for now, it stays for demo perposes
            var prices = competitorPricing.Prices
                .Where(p => p.PricePerKwh.HasValue)
                .Select(p => new PriceResponse(
                    Product: p.Product,
                    Price: p.PricePerKwh!.Value))
                .ToList();

            _logger.LogRetrievedCompetitorPricing(kvkNumber, prices.Count, competitorPricing.Prices.Count);

            return new CompetitorPricingResponse(
                KvkNumber: kvkNumber,
                Prices: prices);
        }
    }
}
