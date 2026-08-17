using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.CompetitorPricingApi;
using Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis.KvkFinderApi;
using Microsoft.Extensions.Options;

namespace Eneco.B2B.CompanyInsights.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddExternalApis(IConfiguration configuration)
            {
                services.AddExternalApiClient<IKvkFinderApiClient, KvkFinderApiClient, KvkFinderApiOptions>(
                    configuration, KvkFinderApiOptions.SectionName);

                services.AddExternalApiClient<ICompetitorPricingApiClient, CompetitorPricingApiClient, CompetitorPricingApiOptions>(
                    configuration, CompetitorPricingApiOptions.SectionName);

                return services;
            }

            private void AddExternalApiClient<TClient, TImplementation, TOptions>(
                IConfiguration configuration,
                string sectionName)
                where TClient : class
                where TImplementation : class, TClient
                where TOptions : class, IExternalApiOptions
            {
                services.Configure<TOptions>(configuration.GetSection(sectionName));

                services.AddHttpClient<TClient, TImplementation>((serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;

                    // The trailing slash keeps relative request URIs from replacing the base path.
                    client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
                    client.DefaultRequestHeaders.Add("X-API-Key", options.ApiKey);
                    client.Timeout = TimeSpan.FromSeconds(options.TotalRequestTimeoutInSeconds);
                });
            }
        }
    }
}
