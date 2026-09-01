using BarbeariaCore.Domain.Entities;

namespace BarbeariaTests.Domain;

public sealed class ServicoTests
{
    [Fact]
    public void Criar_Servico_Valido_Deve_Normalizar_Nome()
    {
        var servico = new Servico("  Corte Masculino  ", 30, 50m, true);
        Assert.Equal("Corte Masculino", servico.Nome);
        Assert.Equal(30, servico.Duracao);
        Assert.Equal(50m, servico.Preco);
        Assert.True(servico.Ativo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Nome_Vazio_Deve_Falhar(string nome)
        => Assert.Equal("SERVICE_INVALID_NAME", Assert.Throws<DomainException>(() => new Servico(nome, 30, 50, true)).Code);

    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    public void Duracao_Invalida_Deve_Falhar(int duracao)
        => Assert.Equal("SERVICE_INVALID_DURATION", Assert.Throws<DomainException>(() => new Servico("Corte", duracao, 50, true)).Code);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Preco_Invalido_Deve_Falhar(decimal preco)
        => Assert.Equal("SERVICE_INVALID_PRICE", Assert.Throws<DomainException>(() => new Servico("Corte", 30, preco, true)).Code);

    [Fact]
    public void AlterarPreco_Valido_Deve_Alterar()
    {
        var s = new Servico("Corte", 30, 50, true);
        s.AlterarPreco(60);
        Assert.Equal(60, s.Preco);
    }

    [Fact]
    public void AlterarPreco_Invalido_Deve_Falhar()
    {
        var s = new Servico("Corte", 30, 50, true);
        Assert.Equal("SERVICE_INVALID_PRICE", Assert.Throws<DomainException>(() => s.AlterarPreco(0)).Code);
    }

    [Fact]
    public void Ativar_Servico_Inativo_Deve_Ativar()
    {
        var s = new Servico("Corte", 30, 50, false);
        s.Ativar();
        Assert.True(s.Ativo);
    }

    [Fact]
    public void Ativar_Servico_Ja_Ativo_Deve_Falhar()
    {
        var s = new Servico("Corte", 30, 50, true);
        Assert.Equal("SERVICE_ALREADY_ACTIVE", Assert.Throws<DomainException>(() => s.Ativar()).Code);
    }

    [Fact]
    public void Desativar_Servico_Ativo_Deve_Desativar()
    {
        var s = new Servico("Corte", 30, 50, true);
        s.Desativar();
        Assert.False(s.Ativo);
    }

    [Fact]
    public void Desativar_Servico_Ja_Inativo_Deve_Falhar()
    {
        var s = new Servico("Corte", 30, 50, false);
        Assert.Equal("SERVICE_ALREADY_INACTIVE", Assert.Throws<DomainException>(() => s.Desativar()).Code);
    }

    [Fact]
    public void AlterarNome_Deve_Trimar()
    {
        var s = new Servico("Corte", 30, 50, true);
        s.AlterarNome("  Barba  ");
        Assert.Equal("Barba", s.Nome);
    }

    [Fact]
    public void AlterarNome_Vazio_Deve_Falhar()
    {
        var s = new Servico("Corte", 30, 50, true);
        Assert.Equal("SERVICE_INVALID_NAME", Assert.Throws<DomainException>(() => s.AlterarNome(" ")).Code);
    }

    [Fact]
    public void AlterarDuracao_Valida_Deve_Alterar()
    {
        var s = new Servico("Corte", 30, 50, true);
        s.AlterarDuracao(60);
        Assert.Equal(60, s.Duracao);
    }

    [Fact]
    public void AlterarDuracao_Invalida_Deve_Falhar()
    {
        var s = new Servico("Corte", 30, 50, true);
        Assert.Equal("SERVICE_INVALID_DURATION", Assert.Throws<DomainException>(() => s.AlterarDuracao(0)).Code);
    }
}

public sealed class BarbeiroTests
{
    [Fact]
    public void Criar_Barbeiro_Valido_Deve_Normalizar_Especialidade()
    {
        var b = new Barbeiro(10, "  Degradê  ");
        Assert.Equal(10, b.UsuarioId);
        Assert.Equal("degradê", b.Especialidade);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UsuarioId_Invalido_Deve_Falhar(int id)
        => Assert.Equal("USER_INVALID_BARBER", Assert.Throws<DomainException>(() => new Barbeiro(id, "corte")).Code);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Especialidade_Vazia_Deve_Falhar(string esp)
        => Assert.Equal("SPECIALTY_INVALID", Assert.Throws<DomainException>(() => new Barbeiro(1, esp)).Code);

    [Fact]
    public void AlterarEspecialidade_Valida_Deve_Normalizar()
    {
        var b = new Barbeiro(1, "corte");
        b.AlterarEspecialidade("  BARBA  ");
        Assert.Equal("barba", b.Especialidade);
    }

    [Fact]
    public void AlterarEspecialidade_Vazia_Deve_Falhar()
    {
        var b = new Barbeiro(1, "corte");
        Assert.Equal("SPECIALTY_INVALID", Assert.Throws<DomainException>(() => b.AlterarEspecialidade(" ")).Code);
    }
}

public sealed class AvaliacaoTests
{
    [Fact]
    public void Criar_Avaliacao_Valida_Deve_Normalizar_Comentario()
    {
        var a = new Avaliacao(1, 2, 3, 5, "  ótimo  ", new DateTime(2026,8,20,10,0,0), 4);
        Assert.Equal(5, a.Nota);
        Assert.Equal("ótimo", a.Comentario);
        Assert.Equal(DateTimeKind.Unspecified, a.DataAtendimento.Kind);
    }

    [Theory]
    [InlineData(0,2,3,4)]
    [InlineData(1,0,3,4)]
    [InlineData(1,2,0,4)]
    [InlineData(1,2,3,0)]
    public void Referencia_Invalida_Deve_Falhar(int barbeiroId, int clienteId, int agendamentoId, int servicoId)
        => Assert.Equal("REVIEW_INVALID_REFERENCE", Assert.Throws<DomainException>(() =>
            new Avaliacao(barbeiroId, clienteId, agendamentoId, 5, null, DateTime.Now, servicoId)).Code);

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Nota_Fora_De_1_A_5_Deve_Falhar(int nota)
        => Assert.Equal("REVIEW_INVALID_SCORE", Assert.Throws<DomainException>(() =>
            new Avaliacao(1,2,3,nota,null,DateTime.Now,4)).Code);

    [Fact]
    public void Comentario_Com_129_Caracteres_Deve_Falhar()
        => Assert.Equal("REVIEW_COMMENT_TOO_LONG", Assert.Throws<DomainException>(() =>
            new Avaliacao(1,2,3,5,new string('a',129),DateTime.Now,4)).Code);

    [Fact]
    public void Comentario_Com_128_Caracteres_Deve_Ser_Aceito()
    {
        var a = new Avaliacao(1,2,3,5,new string('a',128),DateTime.Now,4);
        Assert.Equal(128, a.Comentario!.Length);
    }

    [Fact]
    public void Comentario_Nulo_Deve_Ser_Aceito()
    {
        var a = new Avaliacao(1,2,3,5,null,DateTime.Now,4);
        Assert.Null(a.Comentario);
    }
}
