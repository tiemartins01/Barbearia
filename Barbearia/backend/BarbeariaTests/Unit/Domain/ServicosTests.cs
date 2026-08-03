using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Excepetion;

namespace Barbearia.Tests.Domain;

public class ServicosTests
{
    [Fact]
    public void Deve_Alterar_Preco_Valido()
    {
        var servico = new Servicos("Corte", 30, 50, true);
        servico.AlterarValor(65);
        Assert.Equal(65, servico.Preco);
    }

    [Fact]
    public void Nao_Deve_Aceitar_Preco_Zero()
    {
        var servico = new Servicos("Corte", 30, 50, true);
        Assert.Throws<DomainException>(() => servico.AlterarValor(0));
    }

    [Fact]
    public void Deve_Ativar_E_Desativar_Servico()
    {
        var servico = new Servicos("Corte", 30, 50, false);
        servico.AtivarServico("Corte");
        Assert.True(servico.Ativo);
        servico.DesativarServico("Corte");
        Assert.False(servico.Ativo);
    }
}
