using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi.Contracts;

namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi
{
    public interface IKvkFinderApiClient
    {
        Task<Company> GetCompanyAsync(string kvkNumber, CancellationToken cancellationToken);
    }
}
