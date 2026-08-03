using Barbearia.Observability;
using System.Diagnostics;

namespace Barbearia.Middleware;

public sealed class PerformanceMonitoringMiddleware
{
    private const long SlowRequestThresholdMs = 750;
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;

    public PerformanceMonitoringMiddleware(
        RequestDelegate next,
        ILogger<PerformanceMonitoringMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApiMetrics metrics)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnStarting(() =>
        {
            var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;

            context.Response.Headers["Server-Timing"] =
                $"app;dur={elapsedMs:F2}";

            return Task.CompletedTask;
        });

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;

            var tags = new TagList
            {
                { "http.request.method", context.Request.Method },
                {
                    "http.route",
                    context.GetEndpoint()?.DisplayName
                    ?? context.Request.Path.Value
                    ?? "unknown"
                },
                {
                    "http.response.status_code",
                    context.Response.StatusCode
                }
            };

            metrics.Requests.Add(1, tags);
            metrics.RequestDuration.Record(elapsedMs, tags);

            if (context.Response.StatusCode >=
                StatusCodes.Status500InternalServerError)
            {
                metrics.Errors.Add(1, tags);
            }

            if (stopwatch.ElapsedMilliseconds >= SlowRequestThresholdMs)
            {
                metrics.SlowRequests.Add(1, tags);

                _logger.LogWarning(
                    "Requisição lenta: {Method} {Path} respondeu " +
                    "{StatusCode} em {ElapsedMs} ms. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    context.TraceIdentifier);
            }
        }
    }
}