using Barbearia.HealthChecks;
using Barbearia.Observability;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.ResponseCompression;

namespace BarbeariaApi.Extensions
{
    public static class ObservabilityExtensions
    {

        public static IServiceCollection AddBarbeariaObservability(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ApiMetrics>(); // Coleta métricas automáticas das requisições recebidas pelo ASP.NET Core.
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("Barbearia.Api"))
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation() // Cria traces para chamadas HTTP externas feitas pela API.
                    .AddRuntimeInstrumentation() // Coleta métricas do runtime .NET:
                    .AddMeter("Barbearia.Api")
                    .AddPrometheusExporter())
                .WithTracing(tracing => tracing // Um trace representa o caminho de uma operação.
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = context => !context.Request.Path.StartsWithSegments("/health");
                    })
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        var endpoint = configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317";
                        options.Endpoint = new Uri(endpoint);
                    }));

            services.AddResponseCompression(options => // Reduzir tamanho da resposta
            {
                options.EnableForHttps = true; // Permite compactar respostas mesmo quando a comunicação usa HTTPS
                options.Providers.Add<GzipCompressionProvider>(); // Adiciona Gzip como mecanismo de compressão.
            });

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("Processo em execução."), tags: new[] { "live" })
                .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" });

            return services;
        }

    }
}
