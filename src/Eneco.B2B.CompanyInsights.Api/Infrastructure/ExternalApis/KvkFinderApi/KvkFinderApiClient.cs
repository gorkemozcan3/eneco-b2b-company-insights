using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi.Contracts;

namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi
{
    public sealed class KvkFinderApiClient(HttpClient httpClient) : IKvkFinderApiClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public Task<Company> GetCompanyAsync(string kvkNumber, CancellationToken cancellationToken)
        {
            var requestUri = $"companies/{Uri.EscapeDataString(kvkNumber)}";

            return _httpClient.GetFromJsonOrThrowAsync<Company>(requestUri, cancellationToken);
        }
    }
}
