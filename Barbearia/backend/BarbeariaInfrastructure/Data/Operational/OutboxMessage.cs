namespace Barbearia.Core.Infrastructure.Data.Operational;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTime OccurredAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public int RetryCount { get; private set; }
    public string? LastError { get; private set; }

    private OutboxMessage() { }

    public OutboxMessage(Guid id, string type, string payload, DateTime occurredAtUtc)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredAtUtc = occurredAtUtc;
    }

    public void MarkProcessed(DateTime processedAtUtc)
    {
        ProcessedAtUtc = processedAtUtc;
        LastError = null;
    }

    public void RegisterFailure(string error)
    {
        RetryCount++;
        LastError = error.Length <= 2000 ? error : error[..2000];
    }
}
