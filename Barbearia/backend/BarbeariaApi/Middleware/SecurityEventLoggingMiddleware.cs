namespace Barbearia.Middleware;

public sealed class SecurityEventLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityEventLoggingMiddleware> _logger;

    public SecurityEventLoggingMiddleware(RequestDelegate next, ILogger<SecurityEventLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode is 401 or 403 or 429)
        {
            _logger.LogWarning(
                "Evento de segurança {StatusCode}: {Method} {Path}; IP={Ip}; UserId={UserId}; CorrelationId={CorrelationId}",
                context.Response.StatusCode, context.Request.Method, context.Request.Path,
                context.Connection.RemoteIpAddress?.ToString(),
                context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                context.TraceIdentifier);
        }
    }
}
