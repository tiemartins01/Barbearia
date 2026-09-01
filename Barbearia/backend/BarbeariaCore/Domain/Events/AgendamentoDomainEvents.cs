using BarbeariaCore.Domain.Common;
using BarbeariaCore.Domain.Enum;

namespace BarbeariaCore.Domain.Events;

public sealed record AgendamentoCriadoDomainEvent(
    int AgendamentoId,
    int ClienteId,
    int BarbeiroId,
    int ServicoId,
    DateTime Horario,
    DateTime OccurredAtUtc)
    : IDomainEvent;

public sealed record AgendamentoStatusAlteradoDomainEvent(
    int AgendamentoId,
    StatusAgendamento StatusAnterior,
    StatusAgendamento StatusAtual,
    DateTime OccurredAtUtc)
    : IDomainEvent;