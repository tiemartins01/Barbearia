using System.Text.Json;
using Barbearia.Core.Application.Abstractions;
using Barbearia.Core.Excepetion;
using Barbearia.Core.Infrastructure.Data;
using Barbearia.Core.Infrastructure.Data.Operational;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Barbearia.Core.Infrastructure.Services;

public sealed class DatabaseIdempotencyService : IIdempotencyService
{
    private readonly AppDbContext _context;

    public DatabaseIdempotencyService(AppDbContext context) => _context = context;

    public async Task<IdempotencyExecutionResult<T>> ExecuteAsync<T>(
        string key,
        int userId,
        string operation,
        string requestHash,
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length > 128)
            throw new DomainException("IDEMPOTENCY_KEY_INVALID", "Informe um Idempotency-Key válido com até 128 caracteres.");

        var normalizedKey = key.Trim();
        var existing = await FindAsync(normalizedKey, userId, operation, cancellationToken);

        if (existing is not null)
            return Replay<T>(existing, requestHash);

        var record = new IdempotencyRecord(normalizedKey, userId, operation, requestHash, DateTime.UtcNow);
        _context.IdempotencyRecords.Add(record);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            _context.Entry(record).State = EntityState.Detached;
            existing = await FindAsync(normalizedKey, userId, operation, cancellationToken)
                ?? throw new DomainException("IDEMPOTENCY_CONFLICT", "A operação idempotente já está em processamento.");
            return Replay<T>(existing, requestHash);
        }

        try
        {
            var value = await action();
            record.Complete(JsonSerializer.Serialize(value), DateTime.UtcNow);
            await _context.SaveChangesAsync(cancellationToken);
            return new IdempotencyExecutionResult<T>(value, false);
        }
        catch
        {
            record.Fail();
            await _context.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    private Task<IdempotencyRecord?> FindAsync(
        string key,
        int userId,
        string operation,
        CancellationToken cancellationToken) =>
        _context.IdempotencyRecords.FirstOrDefaultAsync(
            x => x.Key == key && x.UserId == userId && x.Operation == operation,
            cancellationToken);

    private static IdempotencyExecutionResult<T> Replay<T>(IdempotencyRecord record, string requestHash)
    {
        if (!string.Equals(record.RequestHash, requestHash, StringComparison.Ordinal))
            throw new DomainException("IDEMPOTENCY_REQUEST_CONFLICT", "A mesma chave foi reutilizada com dados diferentes.");

        if (record.ExpiresAtUtc <= DateTime.UtcNow)
            throw new DomainException("IDEMPOTENCY_KEY_EXPIRED", "A chave idempotente expirou. Gere uma nova chave.");

        if (record.Status != "Completed" || string.IsNullOrWhiteSpace(record.ResponseBody))
            throw new DomainException("IDEMPOTENCY_IN_PROGRESS", "A operação com esta chave ainda está em processamento.");

        var value = JsonSerializer.Deserialize<T>(record.ResponseBody)
            ?? throw new DomainException("IDEMPOTENCY_RESPONSE_INVALID", "Não foi possível recuperar a resposta idempotente.");

        return new IdempotencyExecutionResult<T>(value, true);
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
}
