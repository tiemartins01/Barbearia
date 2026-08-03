using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Authorization;
using BarbeariaApi.Security;
using Barbearia.BackgroundServices;
using Barbearia.HealthChecks;
using Barbearia.Observability;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using BarbeariaInfrastructure;
using Barbearia.Core.Application.Abstractions;
using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Infrastructure.Data;
using Barbearia.Core.Interface;
using Barbearia.Core.Repository;
using Barbearia.Core.Infrastructure.Services;
using Barbearia.Core.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
namespace BarbeariaApi.Extensions
{
    //Esse método cobre:

    //AppDbContext
    //PostgreSQL
    //connection string
    public static class ServiceCollectionExtensions
    {
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
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
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
            services.AddScoped<IIdempotencyService, DatabaseIdempotencyService>();
            services.AddHostedService<OutboxProcessorService>();

            return services;
        }
        // Application regras dos casos de uso orquestra o fluxo usa interfaces

        //Infrastructure banco persistência serviços externos implementação concreta
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
            services.AddScoped<ITokenService, TokenService>();
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
            //services.AddControllers(options =>
            //{
            //    options.Filters.Add(
            //        new AutoValidateAntiforgeryTokenAttribute()); // AutoValidateAntiforgeryTokenAttribute depende de serviços da infraestrutura de Views/MVC
            //});
            services.AddControllers();
            services.AddProblemDetails();
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
            services.AddHttpContextAccessor();

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
            services.AddScoped<IAuditContext, CurrentAuditContext>();

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
