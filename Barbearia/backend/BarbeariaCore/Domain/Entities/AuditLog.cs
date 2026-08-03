namespace Barbearia.Core.Domain.Entities;

public sealed class AuditLog
{
    public long Id { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
    public int? UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? CorrelationId { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? RequestPath { get; private set; }
    public string? RequestMethod { get; private set; }

    private AuditLog() { }

    public AuditLog(DateTime occurredAtUtc, int? userId, string action, string entityType,
        string entityId, string? oldValues, string? newValues, string? correlationId, string? ipAddress,
        string? userAgent = null, string? requestPath = null, string? requestMethod = null)
    {
        OccurredAtUtc = occurredAtUtc;
        UserId = userId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        OldValues = oldValues;
        NewValues = newValues;
        CorrelationId = correlationId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        RequestPath = requestPath;
        RequestMethod = requestMethod;
    }
}
