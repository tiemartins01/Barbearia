using Barbearia.Core.Domain.ValueObjects;
using Barbearia.Core.Excepetion;

namespace Barbearia.Tests.Domain;

public class ValueObjectsTests
{
    [Fact]
    public void Cpf_Deve_Rejeitar_Sequencia_Repetida()
    {
        var ex = Assert.Throws<DomainException>(() => new Cpf("111.111.111-11"));
        Assert.Equal("USER_INVALID_CPF", ex.Code);
    }

    [Fact]
    public void Phone_Deve_Armazenar_Apenas_Digitos()
    {
        var phone = new Phone("(27) 99999-9999");
        Assert.Equal("27999999999", phone.Telefone);
    }

    [Fact]
    public void Email_Deve_Normalizar_Maiusculas_E_Espacos()
    {
        var email = new Email("  TESTE@EMAIL.COM  ");
        Assert.Equal("teste@email.com", email.EmailPessoa);
    }

    [Fact]
    public void Senha_Nao_Deve_Expor_Hash_No_ToString()
    {
        var senha = Senha.Criar("123456");
        Assert.Equal("********", senha.ToString());
        Assert.True(senha.Verify("123456"));
    }
}
