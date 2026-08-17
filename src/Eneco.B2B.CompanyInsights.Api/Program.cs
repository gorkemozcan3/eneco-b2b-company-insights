using Eneco.B2B.CompanyInsights.Api.Extensions;
using Eneco.B2B.CompanyInsights.Api.Middlewares;
using Eneco.B2B.CompanyInsights.Api.Services;
using Eneco.B2B.CompanyInsights.Api.Services.Interfaces;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddExternalApis(builder.Configuration);

builder.Services.AddScoped<ICompanyService, CompanyService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignore properties with null values from response payloads.
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.ConfigureHttpClientDefaults(builder => builder.AddStandardResilienceHandler(options =>
{
    // can be configured via appsettings.json as well, these are just demonstration values
    options.Retry.MaxRetryAttempts = 3;
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
}));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi("CompanyInsightsApi");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Exposed so the integration tests can bootstrap the application with WebApplicationFactory.
/// </summary>
public partial class Program;
