using Barbearia.HealthChecks;
using Barbearia.Middleware;
using BarbeariaApi.Extensions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics; //  permite que sua API registre informações como: Quantas requisições foram feitas. Quanto tempo cada requisição demorou. Uso de CPU e memória. Número de exceções. Quantidade de conexões com banco. Métricas personalizadas da aplicação. 
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
    .AddRateLimiting()
    .AddBarbeariaObservability(builder.Configuration)
    .AddBarbeariaEmail(builder.Configuration)
    .AddBarbeariaCors(builder.Configuration)
    .AddBarbeariaAuthentication(builder.Configuration).
    AddBarbeariaApiServices();

var app = builder.Build();

// Normalmente carrega : appsettings.json appsettings.Development.json variáveis de ambiente user secrets argumentos da linha de comando


app.UseMiddleware<ErrorHandlingMiddleware>(); // É o middleware responsável por capturar exceções que acontecerem durante uma requisição. Evita try catch em todos os controllers.
app.UseMiddleware<RequestLoggingMiddleware>(); // Registra informações sobre cada requisição recebida, até as corretas
app.UseMiddleware<SecurityEventLoggingMiddleware>(); // Registra os acontecimentos de segurança
app.UseMiddleware<PerformanceMonitoringMiddleware>(); // Mede quanto tempo cada requisição demora.
app.UseMiddleware<SecurityHeadersMiddleware>(); // Adiciona headers de segurança às respostas HTTP.
app.UseResponseCompression(); // Ativa a compactação das respostas. -> Reduzir tamanho

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection(); // Redireciona uma requisição HTTP para HTTPS.
}

app.UseRouting(); // Analisa a requisição e identifica qual endpoint corresponde à rota.
//Exemplo:
//POST / login

//O roteamento procura algo como:

//[Route("login")]
//[HttpPost]
app.UseCors("AllowReact");

app.UseAuthentication(); // É responsável por descobrir quem está fazendo a requisição.
app.UseRateLimiter();
//Requisição chega
//    ↓
//UseAuthentication procura o cookie access-token
//    ↓
//Lê o JWT
//    ↓
//Valida assinatura
//    ↓
//Valida emissor
//    ↓
//Valida audiência
//    ↓
//Valida expiração
//    ↓
//Preenche HttpContext.User

app.UseAuthorization(); // Determina se o usuário autenticado possui permissão para acessar o endpoint.

// Ordem tem que ser primeiro UseAuthentication e depois UseAuthorization

// HealthCheckOptions -> Essa classe configura como os endpoints de saúde devem selecionar e apresentar os checks.
// Ele verifica se o processo da API está vivo. "Se a API conseguiu executar esse código, então o processo está vivo."
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync
});
// A aplicação está pronta para receber requisições reais?
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync
});

//Exemplo:
//API rodando: sim
//Banco conectado: não

//Resultado:
/// health / live  → Healthy
/// health / ready → Unhealthy

app.MapPrometheusScrapingEndpoint("/metrics"); // Esse endpoint expõe as métricas para o Prometheus.
app.MapControllers();
app.Run();

public partial class Program;
