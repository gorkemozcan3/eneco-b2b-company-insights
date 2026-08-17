using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi.Contracts;
using Microsoft.AspNetCore.WebUtilities;

namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi
{
    public sealed class CompetitorPricingApiClient(HttpClient httpClient) : ICompetitorPricingApiClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public Task<CompetitorPricingInfo> GetCompetitorPricingAsync(string kvkNumber, CancellationToken cancellationToken)
        {
            var requestUri = QueryHelpers.AddQueryString("pricing", "kvk", kvkNumber);

            return _httpClient.GetFromJsonOrThrowAsync<CompetitorPricingInfo>(requestUri, cancellationToken);
        }
    }
}
