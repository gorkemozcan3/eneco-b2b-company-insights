namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi.Contracts
{
    public sealed record Company(
        string KvkNumber,
        string Name,
        CompanyAddress Address,
        string Industry);
}
