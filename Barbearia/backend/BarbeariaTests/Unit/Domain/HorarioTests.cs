using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Enum;
using Barbearia.Core.Exceptions;

namespace Barbearia.Tests.Domain;

public class HorariosTests
{

    [Fact]
    public void Novo_Agendamento_Deve_Iniciar_Agendado()
    {
        var horarioFuturo = DateTime.Now.AddDays(1);
        var horario = new Horarios(1, 2, 1, horarioFuturo);
        Assert.Equal(StatusAgendamento.Agendado, horario.StatusAgendamento);
        Assert.Equal(DateTimeKind.Unspecified, horario.Horario.Kind);
    }

    [Fact]
    public void Somente_Concluido_Pode_Ser_Avaliado()
    {
        var horarioFuturo = DateTime.Now.AddDays(1);
        var horario = new Horarios(1, 2, 1, horarioFuturo);
        Assert.Throws<DomainException>(() => horario.Avaliado());
        horario.Concluir();
        horario.Avaliado();
        Assert.Equal(StatusAgendamento.Avaliado, horario.StatusAgendamento);
    }
}
