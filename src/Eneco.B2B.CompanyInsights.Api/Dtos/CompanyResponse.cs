namespace Eneco.B2B.CompanyInsights.Api.Dtos
{
    public record CompanyResponse(
      string KvkNumber,
      string CompanyName,
      string PostalCode,
      string City,
      string Industry);
}
