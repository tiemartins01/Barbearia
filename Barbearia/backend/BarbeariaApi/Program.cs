using OpenTelemetry.Metrics;
using Barbearia.HealthChecks;
using Barbearia.Middleware;
using BarbeariaApi.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddBarbeariaDatabase(builder.Configuration)
    .AddBarbeariaApplication()
    .AddBarbeariaInfrastructure()
    .AddBarbeariaApiServices(builder.Configuration)
    .AddBarbeariaObservability(builder.Configuration)
    .AddRateLimiting()
    .AddBarbeariaEmail(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<SecurityEventLoggingMiddleware>();
app.UseMiddleware<PerformanceMonitoringMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("AllowReact");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync
});

app.MapPrometheusScrapingEndpoint("/metrics");
app.MapControllers();
app.Run();

public partial class Program;
