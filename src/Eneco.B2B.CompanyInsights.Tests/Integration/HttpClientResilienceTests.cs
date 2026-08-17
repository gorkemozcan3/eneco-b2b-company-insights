using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi;
using Eneco.B2B.CompanyInsights.Tests.TestStubs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Eneco.B2B.CompanyInsights.Tests.Integration
{
    public class HttpClientResilienceTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string ValidKvkNumber = "12345679";

        private const string CompanyJson = """
            {
                "kvkNumber": "12345679",
                "name": "Eneco B.V.",
                "address": {
                    "street": "Marten Meesweg",
                    "houseNumber": "5",
                    "postalCode": "3068AV",
                    "city": "Rotterdam"
                },
                "industry": "Energy"
            }
            """;

        private readonly WebApplicationFactory<Program> _factory = factory;

        [Fact]
        public async Task GetCompanyAsync_FirstAttemptFailsWithServerError_RetriesAndReturnsCompany()
        {
            // Arrange
            var handlerStub = HttpMessageHandlerStub.WithResponseSequence(
                new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
                HttpMessageHandlerStub.CreateJsonResponse(CompanyJson));

            using var host = _factory.WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                    services.ConfigureHttpClientDefaults(client =>
                        client.ConfigurePrimaryHttpMessageHandler(() => handlerStub))));

            var kvkFinderApiClient = host.Services.GetRequiredService<IKvkFinderApiClient>();

            // Act
            var company = await kvkFinderApiClient.GetCompanyAsync(ValidKvkNumber, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(2, handlerStub.ReceivedRequests.Count);
            Assert.Equal("Eneco B.V.", company.Name);
        }
    }
}
