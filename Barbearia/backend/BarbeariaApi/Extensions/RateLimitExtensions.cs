using System.Security.Claims;
using System.Threading.RateLimiting;

namespace BarbeariaApi.Extensions
{
    public static class RateLimitExtensions
    {

        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {

            services.AddRateLimiter(options =>
            {
                // Retorno quando se tenta 5x consecutivas
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("login", context =>
                {
                    var ip = ObterIp(context);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"login: {ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("cadastro", context =>
                {
                    var ip = ObterIp(context);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"cadastro:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(5),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("recuperacao-senha", context =>
                {
                    var ip = ObterIp(context);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"recuperacao:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("troca-senha", context =>
                {
                    var identificador = ObterUsuarioOuIp(context);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"troca-senha:{identificador}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("refresh", context =>
                {
                    var identificador = ObterUsuarioOuIp(context);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"refresh:{identificador}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    context.HttpContext.Response.ContentType = "application/json; charset=utf-8";

                    await context.HttpContext.Response.WriteAsJsonAsync(
                        new
                        {
                            sucesso = false,
                            codigo = "RATE_LIMIT_EXCEEDED",
                            mensagem = "Muitas requisições. Tente novamente mais tarde.",
                            traceid = context.HttpContext.TraceIdentifier
                        }, cancellationToken);
                };

            });
            return services;
        }

        private static string ObterIp(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "ip-desconhecido";
        }

        private static string ObterUsuarioOuIp(HttpContext context)
        {
            var userid = context.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userid))
                return $"usuario;{userid}";

            return $"ip {ObterIp(context)}";
        }

    }
}
