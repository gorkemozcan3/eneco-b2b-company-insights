namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi.Contracts
{
    public sealed record CompanyAddress(
        string Street,
        string HouseNumber,
        string PostalCode,
        string City);
}
