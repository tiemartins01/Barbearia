using BarbeariaCore.Domain.Common;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.Events;
using BarbeariaCore.Domain.Exceptions;
using BarbeariaCore.Domain.Policies;

namespace BarbeariaCore.Domain.Entities;

public sealed class Agendamento : AggregateRoot
{
    public int Id { get; private set; }

    public int ClienteId { get; private set; }

    public Usuario Cliente { get; private set; } =
        null!;

    public int BarbeiroId { get; private set; }

    public Barbeiro Barbeiro { get; private set; } =
        null!;

    public int ServicoId { get; private set; }

    public Servico Servico { get; private set; } =
        null!;

    public int DuracaoMinutos { get; private set; }

    public DateTime DataAgendamento
    {
        get;
        private set;
    }

    public DateTime HorarioFim
    {
        get;
        private set;
    }

    public StatusAgendamento Status
    {
        get;
        private set;
    }

    private Agendamento()
    {
    }

    public Agendamento(
        int clienteId,
        int barbeiroId,
        int servicoId,
        int duracaoMinutos,
        DateTime dataAgendamento,
        DateTime agora)
    {
        ValidarCliente(clienteId);

        ValidarBarbeiro(barbeiroId);

        ValidarServico(servicoId);

        PoliticaAgenda.ValidarDuracao(
            duracaoMinutos);

        PoliticaAgenda.ValidarHorarioFuturo(
            dataAgendamento,
            agora);

        PoliticaAgenda.ValidarHorarioNaGrade(
            dataAgendamento);

        PoliticaAgenda
            .ValidarTerminoDentroDoExpediente(
                dataAgendamento,
                duracaoMinutos);

        ClienteId = clienteId;

        BarbeiroId = barbeiroId;

        ServicoId = servicoId;

        DuracaoMinutos = duracaoMinutos;

        DataAgendamento =
            DateTime.SpecifyKind(
                dataAgendamento,
                DateTimeKind.Unspecified);

        HorarioFim =
            DataAgendamento.AddMinutes(
                DuracaoMinutos);

        Status =
            StatusAgendamento.Agendado;
    }

    public void RegistrarCriacao(
        DateTime ocorridoEmUtc)
    {
        if (Id <= 0)
        {
            throw new DomainException(
                "APPOINTMENT_INVALID_ID",
                "Agendamento ainda não foi persistido.");
        }

        AddDomainEvent(
            new AgendamentoCriadoDomainEvent(
                Id,
                ClienteId,
                BarbeiroId,
                ServicoId,
                DataAgendamento,
                ocorridoEmUtc));
    }

    public void Concluir(
        DateTime ocorridoEmUtc)
    {
        AlterarStatus(
            StatusAgendamento.Concluido,
            StatusAgendamento.Agendado,
            "Somente um agendamento ativo pode ser concluído.",
            ocorridoEmUtc);
    }

    public void Cancelar(
        DateTime ocorridoEmUtc)
    {
        AlterarStatus(
            StatusAgendamento.Cancelado,
            StatusAgendamento.Agendado,
            "Somente um agendamento ativo pode ser cancelado.",
            ocorridoEmUtc);
    }

    public void MarcarComoAvaliado(
        DateTime ocorridoEmUtc)
    {
        AlterarStatus(
            StatusAgendamento.Avaliado,
            StatusAgendamento.Concluido,
            "Somente um atendimento concluído pode ser avaliado.",
            ocorridoEmUtc,
            "REVIEW_INVALID_APPOINTMENT_STATUS");
    }

    private void AlterarStatus(
        StatusAgendamento novoStatus,
        StatusAgendamento statusEsperado,
        string mensagem,
        DateTime ocorridoEmUtc,
        string codigo =
            "APPOINTMENT_INVALID_STATUS")
    {
        if (Status != statusEsperado)
        {
            throw new DomainException(
                codigo,
                mensagem);
        }

        var statusAnterior =
            Status;

        Status =
            novoStatus;

        AddDomainEvent(
            new AgendamentoStatusAlteradoDomainEvent(
                Id,
                statusAnterior,
                novoStatus,
                ocorridoEmUtc));
    }

    private static void ValidarCliente(
        int clienteId)
    {
        if (clienteId <= 0)
        {
            throw new DomainException(
                "APPOINTMENT_INVALID_CLIENT",
                "Cliente inválido.");
        }
    }

    private static void ValidarBarbeiro(
        int barbeiroId)
    {
        if (barbeiroId <= 0)
        {
            throw new DomainException(
                "APPOINTMENT_INVALID_BARBER",
                "Barbeiro inválido.");
        }
    }

    private static void ValidarServico(
        int servicoId)
    {
        if (servicoId <= 0)
        {
            throw new DomainException(
                "APPOINTMENT_INVALID_SERVICE",
                "Serviço inválido.");
        }
    }
}