using BarbeariaCore.Domain.Common;

namespace BarbeariaCore.Domain.Events;

public sealed record UsuarioCriadoDomainEvent(
    int UsuarioId,
    string Login,
    DateTime OccurredAtUtc)
    : IDomainEvent;

public sealed record UsuarioBloqueadoDomainEvent(
    int UsuarioId,
    DateTime BloqueioAte,
    DateTime OccurredAtUtc)
    : IDomainEvent;

public sealed record SenhaAlteradaDomainEvent(
    int UsuarioId,
    DateTime OccurredAtUtc)
    : IDomainEvent;

public sealed record RecuperacaoSenhaSolicitadaDomainEvent(
    int UsuarioId,
    DateTime ExpiraEm,
    DateTime OccurredAtUtc)
    : IDomainEvent;

public sealed record UsuarioAtivacaoAlteradaDomainEvent(
    int UsuarioId,
    bool Ativado,
    DateTime OccurredAtUtc)
    : IDomainEvent;
