using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Barbearia.Middleware;

public sealed class RequestLoggingMiddleware
{
    private const string CorrelationHeader = "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        var stopwatch = Stopwatch.StartNew();

        context.TraceIdentifier = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationHeader] = correlationId;
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["TraceId"] = Activity.Current?.TraceId.ToString() ?? correlationId
        }))
        {
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation(
                    "HTTP {Method} {Path} respondeu {StatusCode} em {ElapsedMs} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        var receivedCorrelationId =
            context.Request.Headers[CorrelationHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(receivedCorrelationId))
            return Guid.NewGuid().ToString("N");

        var correlationId = receivedCorrelationId.Trim();

        if (!CorrelationRegex.IsMatch(correlationId))
            return Guid.NewGuid().ToString("N");

        return correlationId;
    }
    // Formato permitido para X-correlation-ID
    // Máximo de 64 caracteres
    private static readonly Regex CorrelationRegex =
        new("^[a-zA-Z0-9-_]{1,64}$");

   
}