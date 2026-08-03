namespace Barbearia.Core.Application.Abstractions;

public sealed record IdempotencyExecutionResult<T>(T Value, bool Replayed);

public interface IIdempotencyService
{
    Task<IdempotencyExecutionResult<T>> ExecuteAsync<T>(
        string key,
        int userId,
        string operation,
        string requestHash,
        Func<Task<T>> action,
        CancellationToken cancellationToken = default);
}
