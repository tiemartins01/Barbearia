using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Excepetion;

namespace Barbearia.Tests.Domain;

public class AvaliacoesTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Deve_Rejeitar_Nota_Fora_Do_Intervalo(int nota)
    {
        var ex = Assert.Throws<DomainException>(() =>
            new Avaliacoes(1, 1, 1, nota, null, DateTime.Now, 1));

        Assert.Equal("REVIEW_INVALID_SCORE", ex.Code);
    }

    [Fact]
    public void Deve_Normalizar_Comentario_E_Data()
    {
        var avaliacao = new Avaliacoes(1, 1, 1, 5, "  excelente  ", DateTime.UtcNow, 1);

        Assert.Equal("excelente", avaliacao.Comentario);
        Assert.Equal(DateTimeKind.Unspecified, avaliacao.Horario.Kind);
    }

    [Fact]
    public void Deve_Rejeitar_Comentario_Maior_Que_128_Caracteres()
    {
        var ex = Assert.Throws<DomainException>(() =>
            new Avaliacoes(1, 1, 1, 5, new string('a', 129), DateTime.Now, 1));

        Assert.Equal("REVIEW_COMMENT_TOO_LONG", ex.Code);
    }
}
