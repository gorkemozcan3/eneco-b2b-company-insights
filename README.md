# Eneco B2B Company Insights — KVK Lookup & Competitor Pricing API

A small Web API built on .NET 10 that exposes two read endpoints for the B2B front-end:
company details by KVK number, and competitor pricing using the KVK number input. The API validates
requests, calls two external providers, and translates their responses into stable contracts
owned by this service.

---

## Quick start

Requires the **.NET 10 SDK**. Run the command below from the repository root to start the API in Development mode:

```shell
dotnet run --project src/Eneco.B2B.CompanyInsights.Api/
```

The bound URLs are printed on startup and come from
`src/Eneco.B2B.CompanyInsights.Api/Properties/launchSettings.json` (currently `https://localhost:7128`
and `http://localhost:5187`).

In Development, interactive API documentation is served by **Scalar** at `https://localhost:7128/scalar/CompanyInsightsApi`, 
backed by the OpenAPI document at `https://localhost:7128/openapi/CompanyInsightsApi.json`.

> **The provider URLs supplied by the assignment are fictional.** Calls against the default
> provider URLs will fail. The automated tests use controlled HTTP stubs instead, so they do not
> depend on any live service.

---

## Running the tests

```shell
dotnet test
```

Unit and integration tests cover input validation, contract mapping, provider failures, 
error semantics, retry behaviour, and the HTTP surface. External HTTP dependencies are replaced with controlled stubs.

---

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/companies/{kvkNumber}` | Registered company details for a KVK number. |
| `GET` | `/api/companies/{kvkNumber}/competitor-pricing` | Competitor pricing using the KVK number input. |

`kvkNumber` must contain exactly 8 ASCII digits.


### Responses

Example successful response used by tests/stubs.

GET /api/companies/12345679
```json
{
  "kvkNumber": "12345679",
  "companyName": "Example BV",
  "postalCode": "1234AB",
  "city": "Rotterdam",
  "industry": "Retail"
}
```

GET /api/companies/12345679/competitor-pricing
```json
{
  "kvkNumber": "12345679",
  "prices": [
	{ "product": "Electricity SME", "price": 0.27 }
  ]
}
```

### Status codes

`200`, `400` (invalid KVK number), `404`, `500`, `502` (provider unreachable or unusable) and
`504` (provider timeout). All error responses follow RFC 9457 `ProblemDetails`; the reasoning
behind the mapping is in the [design document](docs/design-document.md).

---

## Configuration

Per-provider settings are bound with the options pattern from `appsettings.json`, and can be
overridden per environment via `appsettings.{Environment}.json`, environment variables, or user
secrets.

```json
"KvkFinderApi": {
  "BaseUrl": "https://api.external-kvk.com",
  "ApiKey": "replace-me",
  "TotalRequestTimeoutInSeconds": 30
},
"CompetitorPricingApi": {
  "BaseUrl": "https://api.competify.com",
  "ApiKey": "replace-me",
  "TotalRequestTimeoutInSeconds": 30
}
```

| Setting | Purpose |
| --- | --- |
| `BaseUrl` | Provider base address. |
| `ApiKey` | Sent as the `X-API-Key` request header. |
| `TotalRequestTimeoutInSeconds` | Hard upper bound for the whole operation, retries included. |

> **Secrets.** The API keys committed here are **placeholders for local development only**.
> Production credentials belong in the organization’s secret-management platform, such as Azure Key Vault with managed identity.

---

## Repository structure

```
src/
  Eneco.B2B.CompanyInsights.Api/
    Controllers/       HTTP endpoints
    Services/          Use-case orchestration and contract mapping
    Infrastructure/    External-provider clients and contracts
  Eneco.B2B.CompanyInsights.Tests/
    Unit/
    Integration/
    TestStubs/
docs/
  design-document.md
  ai-usage-statement.md
```

The production application is a **single deployable project**, with layering enforced by namespaces
and interface boundaries; tests live in a separate test project. The rationale — and the trigger
for splitting the application further — is in the design document.

---

## Design and assumptions

- [Design document](docs/design-document.md) — architecture, data flow, error policy, trade-offs.
- [AI usage statement](docs/ai-usage-statement.md) — AI tooling usage and how their output was validated.

---

## Limitations

- The external providers are fictional; the service cannot be exercised end to end against them.
- The public pricing contract carries no unit or currency, so the current electricity-only
  interpretation is provisional and remains an open question.
- No persistence, caching, authentication, traffic controls or telemetry — these need production
  requirements before a specific solution is chosen.
