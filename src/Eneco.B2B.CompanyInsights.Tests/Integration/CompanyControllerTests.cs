using Eneco.B2B.CompanyInsights.Api.Dtos;
using Eneco.B2B.CompanyInsights.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Eneco.B2B.CompanyInsights.Tests.Integration
{
    public class CompanyControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string ValidKvkNumber = "12345679";
        private const string InvalidKvkNumber = "1234567";

        private readonly ICompanyService _companyService = Substitute.For<ICompanyService>();
        private readonly WebApplicationFactory<Program> _factory;

        public CompanyControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                    services.AddScoped(_ => _companyService)));
        }

        [Fact]
        public async Task GetCompanyByKvkNumber_ValidKvkNumber_ReturnsOkWithCompany()
        {
            // Arrange
            _companyService.GetCompanyAsync(ValidKvkNumber, Arg.Any<CancellationToken>()).Returns(new CompanyResponse(
                KvkNumber: ValidKvkNumber,
                CompanyName: "Eneco B.V.",
                PostalCode: "3068AV",
                City: "Rotterdam",
                Industry: "Energy"));

            using var client = _factory.CreateClient();

            // Act
            using var response = await client.GetAsync($"/api/companies/{ValidKvkNumber}", TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var company = await response.Content.ReadFromJsonAsync<CompanyResponse>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal("Eneco B.V.", company.CompanyName);
            Assert.Equal(ValidKvkNumber, company.KvkNumber);
        }

        [Fact]
        public async Task GetCompanyByKvkNumber_InvalidKvkNumber_ReturnsBadRequestWithValidationProblemDetails()
        {
            // Arrange
            using var client = _factory.CreateClient();

            // Act
            using var response = await client.GetAsync($"/api/companies/{InvalidKvkNumber}", TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problemDetails);
            Assert.Contains("kvkNumber", problemDetails!.Errors.Keys);

            await _companyService.DidNotReceive().GetCompanyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetCompetitorPricingByKvkNumber_KvkFinderApiFails_ReturnsBadGatewayProblemDetails()
        {
            // Arrange
            _companyService.GetCompetitorPricingAsync(ValidKvkNumber, Arg.Any<CancellationToken>())
                .Returns<CompetitorPricingResponse>(_ => throw new HttpRequestException("upstream down"));

            using var client = _factory.CreateClient();

            // Act
            using var response = await client.GetAsync($"/api/companies/{ValidKvkNumber}/competitor-pricing", TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problemDetails);
            Assert.Equal("An external API could not be reached.", problemDetails!.Title);
        }
    }
}
