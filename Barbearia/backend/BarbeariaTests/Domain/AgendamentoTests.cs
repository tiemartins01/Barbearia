using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.Events;
using BarbeariaTests.Helpers;

namespace BarbeariaTests.Domain;

public sealed class AgendamentoTests
{
    private static DateTime Agora =>
        new(
            2026,
            8,
            28,
            8,
            0,
            0,
            DateTimeKind.Unspecified);

    private static DateTime Horario =>
        new(
            2026,
            8,
            29,
            10,
            0,
            0,
            DateTimeKind.Unspecified);

    [Theory]
    [InlineData(0, 1, 1, "APPOINTMENT_INVALID_CLIENT")]
    [InlineData(1, 0, 1, "APPOINTMENT_INVALID_BARBER")]
    [InlineData(1, 1, 0, "APPOINTMENT_INVALID_SERVICE")]
    public void Referencias_Invalidas_Devem_Falhar(
        int cliente,
        int barbeiro,
        int servico,
        string code)
    {
        var ex = Assert.Throws<DomainException>(
            () => new Agendamento(
                cliente,
                barbeiro,
                servico,
                30,
                Horario,
                Agora));

        Assert.Equal(code, ex.Code);
    }

    [Fact]
    public void Duracao_Invalida_Deve_Falhar()
    {
        var ex = Assert.Throws<DomainException>(
            () => new Agendamento(
                1,
                1,
                1,
                45,
                Horario,
                Agora));

        Assert.Equal(
            "APPOINTMENT_INVALID_DURATION",
            ex.Code);
    }

    [Fact]
    public void Horario_Passado_Deve_Falhar()
    {
        var ex = Assert.Throws<DomainException>(
            () => new Agendamento(
                1,
                1,
                1,
                30,
                Agora.AddMinutes(-30),
                Agora));

        Assert.Equal(
            "APPOINTMENT_DATE_INVALID",
            ex.Code);
    }

    [Fact]
    public void Horario_Fora_Da_Grade_Deve_Falhar()
    {
        var horarioInvalido = new DateTime(
            2026,
            8,
            29,
            10,
            15,
            0,
            DateTimeKind.Unspecified);

        var ex = Assert.Throws<DomainException>(
            () => new Agendamento(
                1,
                1,
                1,
                30,
                horarioInvalido,
                Agora));

        Assert.Equal(
            "APPOINTMENT_INVALID_TIME_SLOT",
            ex.Code);
    }

    [Fact]
    public void Atendimento_Que_Ultrapassa_Expediente_Deve_Falhar()
    {
        var horario = new DateTime(
            2026,
            8,
            29,
            17,
            30,
            0,
            DateTimeKind.Unspecified);

        var ex = Assert.Throws<DomainException>(
            () => new Agendamento(
                1,
                1,
                1,
                60,
                horario,
                Agora));

        Assert.Equal(
            "APPOINTMENT_EXCEEDS_BUSINESS_HOURS",
            ex.Code);
    }

    [Fact]
    public void Criacao_Valida_Deve_Definir_Status_E_Fim()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            60,
            Horario,
            Agora);

        Assert.Equal(
            StatusAgendamento.Agendado,
            a.Status);

        Assert.Equal(
            Horario,
            a.DataAgendamento);

        Assert.Equal(
            Horario.AddMinutes(60),
            a.HorarioFim);

        Assert.Equal(
            DateTimeKind.Unspecified,
            a.DataAgendamento.Kind);
    }

    [Fact]
    public void RegistrarCriacao_Sem_Id_Deve_Falhar()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        var ex = Assert.Throws<DomainException>(
            () => a.RegistrarCriacao(Agora));

        Assert.Equal(
            "APPOINTMENT_INVALID_ID",
            ex.Code);
    }

    [Fact]
    public void RegistrarCriacao_Com_Id_Deve_Gerar_Evento()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        ReflectionHelper.SetPrivateProperty(
            a,
            "Id",
            99);

        a.RegistrarCriacao(Agora);

        var ev = Assert.Single(a.DomainEvents);

        Assert.IsType<AgendamentoCriadoDomainEvent>(ev);
    }

    [Fact]
    public void Agendado_Deve_Poder_Concluir_E_Gerar_Evento()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        a.Concluir(Agora);

        Assert.Equal(
            StatusAgendamento.Concluido,
            a.Status);

        Assert.Contains(
            a.DomainEvents,
            x => x is AgendamentoStatusAlteradoDomainEvent);
    }

    [Fact]
    public void Agendado_Deve_Poder_Cancelar_E_Gerar_Evento()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        a.Cancelar(Agora);

        Assert.Equal(
            StatusAgendamento.Cancelado,
            a.Status);

        Assert.Contains(
            a.DomainEvents,
            x => x is AgendamentoStatusAlteradoDomainEvent);
    }

    [Fact]
    public void Cancelado_Nao_Deve_Poder_Concluir()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        a.Cancelar(Agora);

        var ex = Assert.Throws<DomainException>(
            () => a.Concluir(Agora));

        Assert.Equal(
            "APPOINTMENT_INVALID_STATUS",
            ex.Code);
    }

    [Fact]
    public void Concluido_Nao_Deve_Poder_Cancelar()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        a.Concluir(Agora);

        var ex = Assert.Throws<DomainException>(
            () => a.Cancelar(Agora));

        Assert.Equal(
            "APPOINTMENT_INVALID_STATUS",
            ex.Code);
    }

    [Fact]
    public void Concluido_Deve_Poder_Ser_Avaliado()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        a.Concluir(Agora);

        a.MarcarComoAvaliado(Agora);

        Assert.Equal(
            StatusAgendamento.Avaliado,
            a.Status);
    }

    [Fact]
    public void Agendado_Nao_Deve_Poder_Ser_Avaliado()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        var ex = Assert.Throws<DomainException>(
            () => a.MarcarComoAvaliado(Agora));

        Assert.Equal(
            "REVIEW_INVALID_APPOINTMENT_STATUS",
            ex.Code);
    }

    [Fact]
    public void Cancelado_Nao_Deve_Poder_Ser_Avaliado()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        a.Cancelar(Agora);

        var ex = Assert.Throws<DomainException>(
            () => a.MarcarComoAvaliado(Agora));

        Assert.Equal(
            "REVIEW_INVALID_APPOINTMENT_STATUS",
            ex.Code);
    }

    [Fact]
    public void ClearDomainEvents_Deve_Limpar_Eventos()
    {
        var a = new Agendamento(
            1,
            2,
            3,
            30,
            Horario,
            Agora);

        a.Concluir(Agora);

        Assert.NotEmpty(a.DomainEvents);

        a.ClearDomainEvents();

        Assert.Empty(a.DomainEvents);
    }
}