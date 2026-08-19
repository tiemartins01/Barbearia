namespace BarbeariaCore.Application.Abstractions;

public interface IAuditContext
{
    int? UserId { get; }
    string? CorrelationId { get; }
    string? IpAddress { get; }
    string? UserAgent { get; }
    string? RequestPath { get; }
    string? RequestMethod { get; }
}
