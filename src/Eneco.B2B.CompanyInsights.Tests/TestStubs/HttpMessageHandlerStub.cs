namespace Eneco.B2B.CompanyInsights.Tests.TestStubs
{
    /// <summary>
    /// A test double for <see cref="HttpMessageHandler"/> that returns preconfigured
    /// responses in order and records the requests it received.
    /// </summary>
    public sealed class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses;
        private readonly HttpResponseMessage _lastResponse;

        public HttpMessageHandlerStub(params HttpResponseMessage[] responses)
        {
            _responses = new Queue<HttpResponseMessage>(responses);
            _lastResponse = responses[^1];
        }

        public List<HttpRequestMessage> ReceivedRequests { get; } = [];

        public static HttpMessageHandlerStub WithJsonResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new HttpMessageHandlerStub(CreateJsonResponse(json, statusCode));
        }

        public static HttpMessageHandlerStub WithStatusCode(HttpStatusCode statusCode)
        {
            return new HttpMessageHandlerStub(new HttpResponseMessage(statusCode));
        }

        public static HttpMessageHandlerStub WithResponseSequence(params HttpResponseMessage[] responses)
        {
            return new HttpMessageHandlerStub(responses);
        }

        public static HttpResponseMessage CreateJsonResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }

        public HttpClient CreateClient(string baseAddress = "https://api.example.com/")
        {
            return new HttpClient(this) { BaseAddress = new Uri(baseAddress) };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Behave like a real handler: never touch the wire once cancellation is requested.
            cancellationToken.ThrowIfCancellationRequested();

            ReceivedRequests.Add(request);

            // Once the configured sequence is exhausted, keep returning the final response.
            var response = _responses.Count > 0 ? _responses.Dequeue() : _lastResponse;

            return Task.FromResult(response);
        }
    }
}
