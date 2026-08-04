using OpenTelemetry.Metrics; //  permite que sua API registre informações como: Quantas requisições foram feitas. Quanto tempo cada requisição demorou. Uso de CPU e memória. Número de exceções. Quantidade de conexões com banco. Métricas personalizadas da aplicação. 
using Barbearia.HealthChecks;
using Barbearia.Middleware;
using BarbeariaApi.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// Observabilidade é a capacidade de entender o que está acontecendo em uma aplicação em produção

//Exemplo de coleta:
//GET / login
//Tempo:
//245 ms
//Status:
//200
//Tamanho da resposta:
//3 KB

// O HealthCheckResponseWriter transforma o resultado dos Health Checks em JSON

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

// Normalmente carrega : appsettings.json appsettings.Development.json variáveis de ambiente user secrets argumentos da linha de comando


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

//HealthCheckOptions -> Essa classe configura como os endpoints de saúde devem selecionar e apresentar os checks.
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
