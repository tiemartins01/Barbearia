using BarbeariaCore.Domain.ValueObjects;

namespace BarbeariaTests.Domain;

public sealed class ValueObjectsTests
{
    [Fact]
    public void Email_Deve_Normalizar_Trim_E_Lowercase()
    {
        var email = new Email("  TESTE@EXEMPLO.COM  ");
        Assert.Equal("teste@exemplo.com", email.Valor);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("a@")]
    [InlineData("@b.com")]
    public void Email_Invalido_Deve_Falhar(string valor)
    {
        var ex = Assert.Throws<DomainException>(() => new Email(valor));
        Assert.Equal("USER_INVALID_EMAIL", ex.Code);
    }

    [Fact]
    public void Emails_Equivalentes_Devem_Ser_Iguais_Por_Valor()
    {
        Assert.Equal(new Email("A@B.COM"), new Email("a@b.com"));
    }

    [Fact]
    public void Cpf_Deve_Remover_Formatacao()
    {
        var cpf = new Cpf("529.982.247-25");
        Assert.Equal("52998224725", cpf.Valor);
    }

    [Fact]
    public void Cpf_Formatado_E_Sem_Formatacao_Devem_Ser_Iguais()
    {
        Assert.Equal(new Cpf("529.982.247-25"), new Cpf("52998224725"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("11111111111")]
    [InlineData("12345678901")]
    [InlineData("52998224724")]
    public void Cpf_Invalido_Deve_Falhar(string valor)
    {
        var ex = Assert.Throws<DomainException>(() => new Cpf(valor));
        Assert.Equal("USER_INVALID_CPF", ex.Code);
    }

    [Fact]
    public void Telefone_Deve_Remover_Formatacao()
    {
        var telefone = new Telefone("(11) 99999-9999");
        Assert.Equal("11999999999", telefone.Valor);
    }

    [Fact]
    public void Telefones_Equivalentes_Devem_Ser_Iguais()
    {
        Assert.Equal(new Telefone("(11) 99999-9999"), new Telefone("11999999999"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1199999999")]
    [InlineData("119999999999")]
    public void Telefone_Que_Nao_Tem_Onze_Digitos_Deve_Falhar(string valor)
    {
        var ex = Assert.Throws<DomainException>(() => new Telefone(valor));
        Assert.Equal("USER_INVALID_PHONE", ex.Code);
    }

    [Fact]
    public void Senha_DeHash_Deve_Armazenar_Hash()
    {
        var senha = Senha.DeHash("hash-valido");
        Assert.Equal("hash-valido", senha.Hash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Senha_Com_Hash_Vazio_Deve_Falhar(string hash)
    {
        var ex = Assert.Throws<DomainException>(() => Senha.DeHash(hash));
        Assert.Equal("USER_INVALID_PASSWORD", ex.Code);
    }

    [Fact]
    public void Senha_ToString_Nao_Deve_Expor_Hash()
    {
        var senha = Senha.DeHash("segredo");
        Assert.Equal("********", senha.ToString());
        Assert.DoesNotContain("segredo", senha.ToString());
    }

    [Fact]
    public void ValueObjects_Iguais_Devem_Funcionar_Em_HashSet()
    {
        var emails = new HashSet<Email> { new("A@B.COM"), new("a@b.com") };
        var cpfs = new HashSet<Cpf> { new("529.982.247-25"), new("52998224725") };
        var telefones = new HashSet<Telefone> { new("(11) 99999-9999"), new("11999999999") };

        Assert.Single(emails);
        Assert.Single(cpfs);
        Assert.Single(telefones);
    }
}
