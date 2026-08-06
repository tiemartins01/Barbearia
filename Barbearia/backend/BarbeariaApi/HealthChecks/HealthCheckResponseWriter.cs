using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Barbearia.HealthChecks;

public static class HealthCheckResponseWriter
{
    //  Transforma o resultado em JSON
    public static Task WriteResponseAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var response = new
        {
            status = report.Status.ToString(),
            durationMs = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                durationMs = Math.Round(entry.Value.Duration.TotalMilliseconds, 2)
            })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
