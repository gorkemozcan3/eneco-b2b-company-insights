using Eneco.B2B.CompanyInsights.Api.Dtos;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi.Contracts;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi.Contracts;
using Eneco.B2B.CompanyInsights.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eneco.B2B.CompanyInsights.Tests.Unit
{
    public class CompanyServiceTests
    {
        private const string ValidKvkNumber = "12345679";

        private readonly IKvkFinderApiClient _kvkFinderApiClient = Substitute.For<IKvkFinderApiClient>();
        private readonly ICompetitorPricingApiClient _competitorPricingApiClient = Substitute.For<ICompetitorPricingApiClient>();
        private readonly CompanyService _sut;

        public CompanyServiceTests()
        {
            _sut = new CompanyService(_kvkFinderApiClient, _competitorPricingApiClient, NullLogger<CompanyService>.Instance);
        }

        [Fact]
        public async Task GetCompanyAsync_ValidKvkNumber_ReturnsMappedCompany()
        {
            // Arrange
            var company = new Company(
                KvkNumber: ValidKvkNumber,
                Name: "Eneco B.V.",
                Address: new CompanyAddress("Marten Meesweg", "5", "3068AV", "Rotterdam"),
                Industry: "Energy");

            _kvkFinderApiClient.GetCompanyAsync(ValidKvkNumber, Arg.Any<CancellationToken>()).Returns(company);

            // Act
            var result = await _sut.GetCompanyAsync(ValidKvkNumber, TestContext.Current.CancellationToken);

            // Assert
            var expected = new CompanyResponse(
                KvkNumber: ValidKvkNumber,
                CompanyName: "Eneco B.V.",
                PostalCode: "3068AV",
                City: "Rotterdam",
                Industry: "Energy");

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetCompanyAsync_ValidKvkNumber_CallsKvkFinderApiWithSameKvkNumber()
        {
            // Arrange
            _kvkFinderApiClient.GetCompanyAsync(ValidKvkNumber, Arg.Any<CancellationToken>()).Returns(new Company(
                KvkNumber: ValidKvkNumber,
                Name: "Eneco B.V.",
                Address: new CompanyAddress("Marten Meesweg", "5", "3068AV", "Rotterdam"),
                Industry: "Energy"));

            // Act
            await _sut.GetCompanyAsync(ValidKvkNumber, TestContext.Current.CancellationToken);

            // Assert
            await _kvkFinderApiClient.Received(1).GetCompanyAsync(ValidKvkNumber, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetCompetitorPricingAsync_MixedPrices_ReturnsOnlyPricesWithPricePerKwh()
        {
            // Arrange
            var pricingInfo = new CompetitorPricingInfo(
                CompanyId: "company-1",
                Prices:
                [
                    new Price("Electricity", PricePerKwh: 0.25m, PricePerM3: null, Currency: "EUR"),
                    new Price("Gas", PricePerKwh: null, PricePerM3: 1.10m, Currency: "EUR"),
                    new Price("Unknown", PricePerKwh: null, PricePerM3: null, Currency: "EUR")
                ]);

            _competitorPricingApiClient.GetCompetitorPricingAsync(ValidKvkNumber, Arg.Any<CancellationToken>()).Returns(pricingInfo);

            // Act
            var result = await _sut.GetCompetitorPricingAsync(ValidKvkNumber, TestContext.Current.CancellationToken);

            // Assert
            var price = Assert.Single(result.Prices);
            Assert.Equal(new PriceResponse("Electricity", 0.25m), price);
        }

        [Fact]
        public async Task GetCompetitorPricingAsync_NoPricesWithPricePerKwh_ReturnsEmptyPriceList()
        {
            // Arrange
            var pricingInfo = new CompetitorPricingInfo(
                CompanyId: "company-1",
                Prices: [new Price("Gas", PricePerKwh: null, PricePerM3: 1.10m, Currency: "EUR")]);

            _competitorPricingApiClient.GetCompetitorPricingAsync(ValidKvkNumber, Arg.Any<CancellationToken>()).Returns(pricingInfo);

            // Act
            var result = await _sut.GetCompetitorPricingAsync(ValidKvkNumber, TestContext.Current.CancellationToken);

            // Assert
            Assert.Empty(result.Prices);
        }

        [Fact]
        public async Task GetCompetitorPricingAsync_ValidKvkNumber_ReturnsRequestedKvkNumber()
        {
            // Arrange
            var pricingInfo = new CompetitorPricingInfo(
                CompanyId: "a-different-company-id",
                Prices: []);

            _competitorPricingApiClient.GetCompetitorPricingAsync(ValidKvkNumber, Arg.Any<CancellationToken>()).Returns(pricingInfo);

            // Act
            var result = await _sut.GetCompetitorPricingAsync(ValidKvkNumber, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(ValidKvkNumber, result.KvkNumber);
        }
    }
}
