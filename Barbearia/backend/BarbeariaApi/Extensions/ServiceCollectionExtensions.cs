using Barbearia.BackgroundServices;
using Barbearia.Core.Application.Abstractions;
using Barbearia.Core.Infrastructure.Data;
using Barbearia.Core.Infrastructure.Services;
using Barbearia.Core.Interface;
using Barbearia.Core.Repository;
using Barbearia.Core.Service;
using Barbearia.HealthChecks;
using Barbearia.Observability;
using BarbeariaApi.Security;
using BarbeariaCore.Application.Interfaces;
using BarbeariaInfrastructure;
using BarbeariaInfrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text;

namespace BarbeariaApi.Extensions
{
    //Esse método cobre:

    //AppDbContext
    //PostgreSQL
    //connection string
    public static class ServiceCollectionExtensions
    {
        // AddBarbeariaDatabase:

        // Busca configuration.GetConnectionString("PostgreSql")
        //depois registra services.AddDbContext<AppDbContext>(...)

        // Caminho da conexão: 
        //Program.cs
        //    ↓
        //AddBarbeariaDatabase
        //    ↓
        //ConnectionStrings:PostgreSql
        //   ↓
        // AppDbContext
        //   ↓
        //Npgsql
        //    ↓
        //PostgreSQL

        // transforma o método em um método de extensão. -> this IServiceCollection
        public static IServiceCollection AddBarbeariaDatabase (this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgreSql")
                ?? throw new InvalidOperationException("A connection string 'PostgreSql' não foi configurada.");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Quantidade máxima de chamadas pós falha
                maxRetryDelay: TimeSpan.FromSeconds(10), // até 10 segundis entre chamadas
                errorCodesToAdd: null);
        });
            });

            return services;
        }

        public static IServiceCollection AddBarbeariaInfrastructure (this IServiceCollection services)
        {
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IAbaClienteRepository, AbaClienteRepository>();
            services.AddScoped<IServicosRepository, ServicosAtivosRepository>();
            services.AddScoped<IEmailEsqueciSenhaRepository, EmailEsqueciSenhaRepository>();
            services.AddScoped<INovoClienteRepository, NovoClienteRepository>();
            services.AddScoped<IProximoAtendimentoRepository, ProximoAtendimentoRepository>();
            services.AddScoped<ITrocaSenhaRepository, TrocaSenhaRepository>();
            services.AddScoped<IRefreshRepository, RefreshRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWorksRepository>();
            services.AddScoped<IPasswordHash, PasswordHasher>();
            services.AddScoped<IIdempotencyService, DatabaseIdempotencyService>();
            services.AddHostedService<OutboxProcessorService>(); //roda em segundo plano
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

        // Application regras dos casos de uso orquestra o fluxo usa interfaces
        // Registro de serviços do Core
        public static IServiceCollection AddBarbeariaApplication(
    this IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IAbaClienteService, AbaClienteService>();
            services.AddScoped<IServicosService, ServicosAtivosService>();
            services.AddScoped<IEmailEsqueciSenhaService, EmailEsqueciSenhaService>();
            services.AddScoped<INovoClienteService, NovoClienteService>();
            services.AddScoped<IProximoAtendimentoService, ProximoAtendimentoService>();
            services.AddScoped<ITrocaSenhaService, TrocaSenhaService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            return services;
        }

        public static IServiceCollection AddBarbeariaObservability(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ApiMetrics>();
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("Barbearia.Api"))
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("Barbearia.Api")
                    .AddPrometheusExporter())
                .WithTracing(tracing => tracing
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

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("Processo em execução."), tags: new[] { "live" })
                .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" });

            return services;
        }

        public static IServiceCollection AddRateLimiting(this  IServiceCollection services)
        {

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter("login", limiter =>
                {
                    limiter.PermitLimit = 5;
                    limiter.Window = TimeSpan.FromMinutes(1);
                    limiter.QueueLimit = 0;
                    limiter.AutoReplenishment = true;
                });
                options.AddFixedWindowLimiter("cadastro", limiter =>
                {
                    limiter.PermitLimit = 5;
                    limiter.Window = TimeSpan.FromMinutes(5);
                    limiter.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter(
                    "recuperacao-senha",
                    limiter =>
                    {
                        limiter.PermitLimit = 3;
                        limiter.Window = TimeSpan.FromMinutes(10);
                        limiter.QueueLimit = 0;
                    });

                options.AddFixedWindowLimiter(
                    "troca-senha",
                    limiter =>
                    {
                        limiter.PermitLimit = 10;
                        limiter.Window = TimeSpan.FromMinutes(10);
                        limiter.QueueLimit = 0;
                    });

                options.AddFixedWindowLimiter("refresh", limiter =>
                {
                    limiter.PermitLimit = 20;
                    limiter.Window = TimeSpan.FromMinutes(1);
                    limiter.QueueLimit = 0;
                });
            });
            return services;
        }

        public static IServiceCollection AddBarbeariaEmail(
    this IServiceCollection services,
    IConfiguration configuration)
        {
            services
        .AddOptions<SmtpSettings>()
        .Bind(configuration.GetSection("SmtpSettings"))
        .Validate(settings =>
                !settings.Enabled || !string.IsNullOrWhiteSpace(settings.Host),
            "SmtpSettings:Host não configurado.")
        .Validate(settings => !settings.Enabled ||
                settings.Port > 0,
            "SmtpSettings:Port inválido.")
        .Validate(settings => !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.FromEmail),
            "SmtpSettings:FromEmail não configurado.")
        .Validate(settings => !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.Username),
            "SmtpSettings:Username não configurado.")
        .Validate(settings => !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.Password),
            "SmtpSettings:Password não configurado.")
        .ValidateOnStart();

            services.AddScoped<IEnviarEmail, EnviarEmail>();

            return services;
        }

        public static IServiceCollection AddBarbeariaApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddProblemDetails(); // registra suporte ao formato padronizado http.
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Barbearia API",
                    Version = "v1",
                    Description = "API REST para autenticação, clientes, serviços e agendamentos."
                });
            });
            services.AddHttpContextAccessor(); // serve para permitir que classes que não são Controllers tenham acesso ao HttpContext da requisição atual
            // Exemplo: Current User

            //configura proteção contra CSRF
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";

                options.Cookie.Name = "XSRF-TOKEN";
                options.Cookie.HttpOnly = false;
                options.Cookie.SecurePolicy =
                    CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });


            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IAuditContext, CurrentAuditContext>(); // Fornece ao AppDbContext : usuário atual; IP; User-Agent; rota; método HTTP; CorrelationId. Verifica quem alterou

            // Criação de um token forte
            var jwtkey = configuration["Jwt:Key"];

            if(string.IsNullOrWhiteSpace(jwtkey) || Encoding.UTF8.GetByteCount(jwtkey) < 32)
            {
                throw new InvalidOperationException("Jwt:Key deve possuir pelo menos 32 bytes.");
            }

            var issuer = configuration["Jwt:Issuer"]
           ?? throw new InvalidOperationException(
               "Jwt:Issuer não configurado.");

            var audience = configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "Jwt:Audience não configurado.");
            // Define JWT Bearer como esquema padrão.

            // Requisição
            //      ↓
            //UseAuthentication()
            //      ↓
            //JWT tenta validar o token
            //      ↓
            //Token inválido ou inexistente
            //      ↓
            //UseAuthorization()
            //      ↓
            //Endpoint exige[Authorize]
            //      ↓
            //DefaultChallengeScheme(JWT Bearer)
            //      ↓
            //Retorna 401 Unauthorized

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Usado para tentar identificar o usuário.
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Usado quando alguém tenta acessar um endpoint protegido sem autenticação válida.
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtkey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["access-token"];

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddScoped<IAuthorizationHandler, ActiveUserHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ActiveUser", policy =>
                    policy.RequireAuthenticatedUser().AddRequirements(new ActiveUserRequirement()));
                options.AddPolicy("ClientOnly", policy =>
                    policy.RequireAuthenticatedUser().RequireRole("Cliente"));
                options.AddPolicy("BarberOnly", policy =>
                    policy.RequireAuthenticatedUser().RequireRole("Barbeiro"));
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireAuthenticatedUser().RequireRole("Admin"));
            });

            var frontendUrl =
           configuration["Frontend:Url"]
           ?? "http://localhost:5173";

            services.AddCors(options =>
            {
                options.AddPolicy("AllowReact", policy =>
                {
                    policy
                        .WithOrigins(frontendUrl)
                        .WithMethods(
                            "GET",
                            "POST",
                            "PUT",
                            "PATCH",
                            "DELETE")
                        .WithHeaders(
                            "Content-Type",
                            "X-CSRF-TOKEN",
                            "Idempotency-Key",
                            "traceparent",
                            "tracestate")
                        .AllowCredentials();
                });
            });

            return services;
        }
    }
}
