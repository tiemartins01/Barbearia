using BarbeariaCore.Domain.Common;

namespace BarbeariaCore.Domain.Events;

public sealed record UsuarioCriadoDomainEvent(int UsuarioId, string Login) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}

public sealed record UsuarioBloqueadoDomainEvent(int UsuarioId, DateTime BloqueioAte) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}

public sealed record SenhaAlteradaDomainEvent(int UsuarioId) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}

public sealed record RecuperacaoSenhaSolicitadaDomainEvent(int UsuarioId, DateTime ExpiraEm) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}

public sealed record UsuarioAtivacaoAlteradaDomainEvent(int UsuarioId, bool Ativado) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
