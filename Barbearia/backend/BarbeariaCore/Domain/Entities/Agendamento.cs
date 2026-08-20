using BarbeariaCore.Domain.Common;
using BarbeariaCore.Domain.Events;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.Exceptions;

namespace BarbeariaCore.Domain.Entities;

/// <summary>
/// Aggregate Root do ciclo de vida de um agendamento.
/// Somente esta entidade pode alterar o status do atendimento.
/// </summary>
public sealed class Agendamento : AggregateRoot
{
    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public Usuario Cliente { get; private set; } = null!;
    public StatusAgendamento Status { get; private set; }
    public int BarbeiroId { get; private set; }
    public Barbeiro Barbeiro { get; private set; } = null!;
    public int ServicoId { get; private set; }
    public Servico Servico { get; private set; } = null!;
    public DateTime Horario { get; private set; }

    private Agendamento() { }

    public Agendamento(int clienteId, int barbeiroId, int servicoID, DateTime horario, DateTime agora)
    {
        if (clienteId <= 0)
            throw new DomainException("AGENDA_INVALID_CLIENT", "Cliente inválido.");
        if (barbeiroId <= 0)
            throw new DomainException("AGENDA_INVALID_BARBER", "Barbeiro inválido.");
        if (servicoID <= 0)
            throw new DomainException("AGENDA_INVALID_SERVICE", "Serviço inválido.");
        if (horario <= agora)
            throw new DomainException("AGENDA_INVALID_DATE", "O agendamento deve ser realizado para um horário futuro.");

        ClienteId = clienteId;
        BarbeiroId = barbeiroId;
        ServicoId = servicoID;
        Horario = DateTime.SpecifyKind(horario, DateTimeKind.Unspecified);
        Status = StatusAgendamento.Agendado;
    }


    public void RegistrarCriacao()
    {
        if(Id <= 0)

            throw new DomainException(
                "AGENDA_INVALID_ID",
                "Agendamento ainda não possui um identificador válido.");

    AddDomainEvent(
            new AgendamentoCriadoDomainEvent(
                Id,
                ClienteId,
                BarbeiroId,
                ServicoId,
                Horario));
    }

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

    public void MarcarComoAvaliado()
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
        if (Status != statusEsperado)
            throw new DomainException(codigo, mensagem);

        var statusAnterior = Status;
        Status = novoStatus;

        AddDomainEvent(new AgendamentoStatusAlteradoDomainEvent(
            Id,
            statusAnterior,
            novoStatus));
    }
}
