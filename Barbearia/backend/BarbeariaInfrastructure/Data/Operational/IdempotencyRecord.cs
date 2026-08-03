namespace Barbearia.Core.Infrastructure.Data.Operational;

public sealed class IdempotencyRecord
{
    public long Id { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public int UserId { get; private set; }
    public string Operation { get; private set; } = string.Empty;
    public string RequestHash { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Processing";
    public string? ResponseBody { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    private IdempotencyRecord() { }

    public IdempotencyRecord(string key, int userId, string operation, string requestHash, DateTime nowUtc)
    {
        Key = key;
        UserId = userId;
        Operation = operation;
        RequestHash = requestHash;
        CreatedAtUtc = nowUtc;
        ExpiresAtUtc = nowUtc.AddHours(24);
    }

    public void Complete(string responseBody, DateTime completedAtUtc)
    {
        ResponseBody = responseBody;
        CompletedAtUtc = completedAtUtc;
        Status = "Completed";
    }

    public void Fail() => Status = "Failed";
}
