using Eneco.B2B.CompanyInsights.Api.Exceptions;
using System.Text.Json;

namespace Eneco.B2B.CompanyInsights.Api.Infrastructure.ExternalApis
{
    public static class HttpClientJsonExtensions
    {
        public static async Task<T> GetFromJsonOrThrowAsync<T>(this HttpClient httpClient, string requestUri, CancellationToken cancellationToken)
        {
            using var response = await httpClient.GetAsync(requestUri, cancellationToken);

            response.EnsureSuccessStatusCode();

            try
            {
                var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);

                if (result is null)
                {
                    throw new ExternalApiException($"The response from '{requestUri}' did not contain a valid {typeof(T).Name}.");
                }

                return result;
            }
            catch (JsonException ex)
            {
                // gracefully catch malformed JSON (e.g., non-JSON text) and return custom exception
                throw new ExternalApiException($"The response from '{requestUri}' was not valid JSON and could not be parsed into {typeof(T).Name}.", ex);
            }
        }
    }
}
