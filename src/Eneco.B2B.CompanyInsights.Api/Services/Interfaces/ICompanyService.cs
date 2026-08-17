using Eneco.B2B.CompanyInsights.Api.Dtos;

namespace Eneco.B2B.CompanyInsights.Api.Services.Interfaces
{
    public interface ICompanyService
    {
        /// <summary>Gets registered company details for a KVK number.</summary>
        Task<CompanyResponse> GetCompanyAsync(string kvkNumber, CancellationToken cancellationToken);

        /// <summary>Gets competitor pricing for the company identified by a KVK number.</summary>
        Task<CompetitorPricingResponse> GetCompetitorPricingAsync(string kvkNumber, CancellationToken cancellationToken);
    }
}
