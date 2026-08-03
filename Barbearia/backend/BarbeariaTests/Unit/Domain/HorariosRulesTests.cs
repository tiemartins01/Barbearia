using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Enum;
using Barbearia.Core.Excepetion;

namespace Barbearia.Tests.Domain;

public class HorariosRulesTests
{
    [Theory]
    [InlineData(0, 1, 1, "AGENDA_INVALID_CLIENT")]
    [InlineData(1, 0, 1, "AGENDA_INVALID_BARBER")]
    [InlineData(1, 1, 0, "AGENDA_INVALID_SERVICE")]
    public void Deve_Rejeitar_Referencias_Invalidas(int cliente, int barbeiro, int servico, string codigo)
    {
        var ex = Assert.Throws<DomainException>(() =>
            new Horarios(cliente, barbeiro, servico, DateTime.Now.AddHours(1)));

        Assert.Equal(codigo, ex.Code);
    }

    [Fact]
    public void Deve_Cancelar_Agendamento_Ativo()
    {
        var horario = new Horarios(1, 1, 1, DateTime.Now.AddHours(1));

        horario.Cancelar();

        Assert.Equal(StatusAgendamento.Cancelado, horario.StatusAgendamento);
    }

    [Fact]
    public void Nao_Deve_Cancelar_Agendamento_Ja_Concluido()
    {
        var horario = new Horarios(1, 1, 1, DateTime.Now.AddHours(1));
        horario.Concluir();

        var ex = Assert.Throws<DomainException>(() => horario.Cancelar());

        Assert.Equal("AGENDA_INVALID_STATUS", ex.Code);
    }

    [Fact]
    public void Nao_Deve_Concluir_Agendamento_Cancelado()
    {
        var horario = new Horarios(1, 1, 1, DateTime.Now.AddHours(1));
        horario.Cancelar();

        var ex = Assert.Throws<DomainException>(() => horario.Concluir());

        Assert.Equal("AGENDA_INVALID_STATUS", ex.Code);
    }
}
