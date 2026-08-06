using System.Diagnostics.Metrics;

namespace Barbearia.Observability;

public sealed class ApiMetrics : IDisposable
{
    private readonly Meter _meter = new("Barbearia.Api", "1.0.0");

    public Counter<long> Requests { get; }
    public Counter<long> Errors { get; }
    public Histogram<double> RequestDuration { get; }
    public Counter<long> SlowRequests { get; }

    //PerformanceMonitoringMiddleware
    //        │
    //        ▼
    //ApiMetrics
    //        │
    //        ▼
    //Meter
    //        │
    //        ▼
    //OpenTelemetry
    //        │
    //        ▼
    ///metrics
    //        │
    //        ▼
    //Prometheus
    //        │
    //        ▼
    //Grafana

    public ApiMetrics()
    {
        Requests = _meter.CreateCounter<long>("barbearia.http.requests");
        Errors = _meter.CreateCounter<long>("barbearia.http.errors");
        RequestDuration = _meter.CreateHistogram<double>("barbearia.http.duration", "ms");
        SlowRequests = _meter.CreateCounter<long>("barbearia.http.slow_requests");
    }

    public void Dispose() => _meter.Dispose();
}
