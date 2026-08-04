using Barbearia.Core.Domain.Common;
using Barbearia.Core.Domain.Events;
using Barbearia.Core.Enum;
using Barbearia.Core.Exceptions;

namespace Barbearia.Core.Domain.Entities;

/// <summary>
/// Aggregate Root do ciclo de vida de um agendamento.
/// Somente esta entidade pode alterar o status do atendimento.
/// </summary>
public sealed class Horarios : AggregateRoot
{
    public int Id { get; private set; }
    public int Id_cliente { get; private set; }
    public Usuario Cliente { get; private set; } = null!;
    public StatusAgendamento StatusAgendamento { get; private set; }
    public int Id_barbeiro { get; private set; }
    public Barbeiro Barbeiro { get; private set; } = null!;
    public int Id_servico { get; private set; }
    public Servicos Servicos { get; private set; } = null!;
    public DateTime Horario { get; private set; }

    private Horarios() { }

    public Horarios(int id_cliente, int id_barbeiro, int id_servico, DateTime horario)
    {
        if (id_cliente <= 0)
            throw new DomainException("AGENDA_INVALID_CLIENT", "Cliente inválido.");
        if (id_barbeiro <= 0)
            throw new DomainException("AGENDA_INVALID_BARBER", "Barbeiro inválido.");
        if (id_servico <= 0)
            throw new DomainException("AGENDA_INVALID_SERVICE", "Serviço inválido.");
        if (horario <= DateTime.Now)
            throw new DomainException("AGENDA_INVALID_DATE", "O agendamento deve ser realizado para um horário futuro.");

        Id_cliente = id_cliente;
        Id_barbeiro = id_barbeiro;
        Id_servico = id_servico;
        Horario = DateTime.SpecifyKind(horario, DateTimeKind.Unspecified);
        StatusAgendamento = StatusAgendamento.Agendado;

        AddDomainEvent(new AgendamentoCriadoDomainEvent(
            Id,
            Id_cliente,
            Id_barbeiro,
            Id_servico,
            Horario));
    }

    public bool HorarioMenor(DateTimeOffset horarioC) => horarioC.LocalDateTime < DateTime.Now;

    public void Concluir()
    {
        AlterarStatus(
            StatusAgendamento.Concluido,
            StatusAgendamento.Agendado,
            "Somente um agendamento ativo pode ser concluído.");
    }

    public void Cancelar()
    {
        AlterarStatus(
            StatusAgendamento.Cancelado,
            StatusAgendamento.Agendado,
            "Somente um agendamento ativo pode ser cancelado.");
    }

    public void Avaliado()
    {
        AlterarStatus(
            StatusAgendamento.Avaliado,
            StatusAgendamento.Concluido,
            "Somente um atendimento concluído pode ser avaliado.",
            "REVIEW_INVALID_APPOINTMENT_STATUS");
    }

    private void AlterarStatus(
        StatusAgendamento novoStatus,
        StatusAgendamento statusEsperado,
        string mensagem,
        string codigo = "AGENDA_INVALID_STATUS")
    {
        if (StatusAgendamento != statusEsperado)
            throw new DomainException(codigo, mensagem);

        var statusAnterior = StatusAgendamento;
        StatusAgendamento = novoStatus;

        AddDomainEvent(new AgendamentoStatusAlteradoDomainEvent(
            Id,
            statusAnterior,
            novoStatus));
    }
}
