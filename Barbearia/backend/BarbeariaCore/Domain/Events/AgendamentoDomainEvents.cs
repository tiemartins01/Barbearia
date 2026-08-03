using Barbearia.Core.Domain.Common;
using Barbearia.Core.Enum;

namespace Barbearia.Core.Domain.Events;

public sealed record AgendamentoCriadoDomainEvent(
    int AgendamentoId,
    int ClienteId,
    int BarbeiroId,
    int ServicoId,
    DateTime Horario) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}

public sealed record AgendamentoStatusAlteradoDomainEvent(
    int AgendamentoId,
    StatusAgendamento StatusAnterior,
    StatusAgendamento StatusAtual) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
