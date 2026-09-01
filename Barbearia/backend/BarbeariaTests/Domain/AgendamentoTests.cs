using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.Events;
using BarbeariaTests.Helpers;

namespace BarbeariaTests.Domain;

public sealed class AgendamentoTests
{
    private static DateTime Agora => new(2026, 8, 28, 8, 0, 0);
    private static DateTime Horario => new(2026, 8, 29, 10, 0, 0);

    [Theory]
    [InlineData(0,1,1,"APPOINTMENT_INVALID_CLIENT")]
    [InlineData(1,0,1,"APPOINTMENT_INVALID_BARBER")]
    [InlineData(1,1,0,"APPOINTMENT_INVALID_SERVICE")]
    public void Referencias_Invalidas_Devem_Falhar(int cliente, int barbeiro, int servico, string code)
        => Assert.Equal(code, Assert.Throws<DomainException>(() => new Agendamento(cliente, barbeiro, servico, 30, Horario, Agora)).Code);

    [Fact]
    public void Duracao_Invalida_Deve_Falhar()
        => Assert.Equal("APPOINTMENT_INVALID_DURATION", Assert.Throws<DomainException>(() => new Agendamento(1,1,1,45,Horario,Agora)).Code);

    [Fact]
    public void Horario_Passado_Deve_Falhar()
        => Assert.Equal("APPOINTMENT_DATE_INVALID", Assert.Throws<DomainException>(() => new Agendamento(1,1,1,30,Agora.AddMinutes(-30),Agora)).Code);

    [Fact]
    public void Horario_Fora_Da_Grade_Deve_Falhar()
        => Assert.Equal("APPOINTMENT_INVALID_TIME_SLOT", Assert.Throws<DomainException>(() => new Agendamento(1,1,1,30,new DateTime(2026,8,29,10,15,0),Agora)).Code);

    [Fact]
    public void Atendimento_Que_Ultrapassa_Expediente_Deve_Falhar()
        => Assert.Equal("APPOINTMENT_EXCEEDS_BUSINESS_HOURS", Assert.Throws<DomainException>(() => new Agendamento(1,1,1,60,new DateTime(2026,8,29,17,30,0),Agora)).Code);

    [Fact]
    public void Criacao_Valida_Deve_Definir_Status_E_Fim()
    {
        var a = new Agendamento(1,2,3,60,Horario,Agora);
        Assert.Equal(StatusAgendamento.Agendado, a.Status);
        Assert.Equal(Horario, a.DataAgendamento);
        Assert.Equal(Horario.AddMinutes(60), a.HorarioFim);
        Assert.Equal(DateTimeKind.Unspecified, a.DataAgendamento.Kind);
    }

    [Fact]
    public void RegistrarCriacao_Sem_Id_Deve_Falhar()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        Assert.Equal("APPOINTMENT_INVALID_ID", Assert.Throws<DomainException>(a.RegistrarCriacao).Code);
    }

    [Fact]
    public void RegistrarCriacao_Com_Id_Deve_Gerar_Evento()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        ReflectionHelper.SetPrivateProperty(a, "Id", 99);
        a.RegistrarCriacao();
        var ev = Assert.Single(a.DomainEvents);
        Assert.IsType<AgendamentoCriadoDomainEvent>(ev);
    }

    [Fact]
    public void Agendado_Deve_Poder_Concluir_E_Gerar_Evento()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        a.Concluir();
        Assert.Equal(StatusAgendamento.Concluido, a.Status);
        Assert.Contains(a.DomainEvents, x => x is AgendamentoStatusAlteradoDomainEvent);
    }

    [Fact]
    public void Agendado_Deve_Poder_Cancelar_E_Gerar_Evento()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        a.Cancelar();
        Assert.Equal(StatusAgendamento.Cancelado, a.Status);
        Assert.Contains(a.DomainEvents, x => x is AgendamentoStatusAlteradoDomainEvent);
    }

    [Fact]
    public void Cancelado_Nao_Deve_Poder_Concluir()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        a.Cancelar();
        Assert.Equal("APPOINTMENT_INVALID_STATUS", Assert.Throws<DomainException>(a.Concluir).Code);
    }

    [Fact]
    public void Concluido_Nao_Deve_Poder_Cancelar()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        a.Concluir();
        Assert.Equal("APPOINTMENT_INVALID_STATUS", Assert.Throws<DomainException>(a.Cancelar).Code);
    }

    [Fact]
    public void Concluido_Deve_Poder_Ser_Avaliado()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        a.Concluir();
        a.MarcarComoAvaliado();
        Assert.Equal(StatusAgendamento.Avaliado, a.Status);
    }

    [Fact]
    public void Agendado_Nao_Deve_Poder_Ser_Avaliado()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        Assert.Equal("REVIEW_INVALID_APPOINTMENT_STATUS", Assert.Throws<DomainException>(a.MarcarComoAvaliado).Code);
    }

    [Fact]
    public void Cancelado_Nao_Deve_Poder_Ser_Avaliado()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        a.Cancelar();
        Assert.Equal("REVIEW_INVALID_APPOINTMENT_STATUS", Assert.Throws<DomainException>(a.MarcarComoAvaliado).Code);
    }

    [Fact]
    public void ClearDomainEvents_Deve_Limpar_Eventos()
    {
        var a = new Agendamento(1,2,3,30,Horario,Agora);
        a.Concluir();
        Assert.NotEmpty(a.DomainEvents);
        a.ClearDomainEvents();
        Assert.Empty(a.DomainEvents);
    }
}
