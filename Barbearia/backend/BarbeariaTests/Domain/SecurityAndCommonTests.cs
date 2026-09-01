using BarbeariaCore.Security;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaCore.Domain.Enum;
using BarbeariaTests.Helpers;

namespace BarbeariaTests.Domain;

public sealed class SecurityAndCommonTests
{
    [Fact]
    public void CodeGenerator_Deve_Gerar_Exatamente_6_Digitos()
    {
        for (var i=0;i<100;i++)
        {
            var code = CodeGenerator.GerarCod();
            Assert.Equal(6, code.Length);
            Assert.True(code.All(char.IsDigit));
        }
    }

    [Fact]
    public void ClearDomainEvents_Deve_Remover_Todos_Eventos()
    {
        var u = new Usuario("Nome",new Email("a@b.com"),new Telefone("11999999999"),new Cpf("52998224725"),"login",Senha.DeHash("hash"),RolePerson.Cliente,true,null);
        ReflectionHelper.SetPrivateProperty(u,"Id",1);
        u.RegistrarCriacao();
        u.AlterarSenhaPerfil(Senha.DeHash("novo"));
        Assert.Equal(2,u.DomainEvents.Count);
        u.ClearDomainEvents();
        Assert.Empty(u.DomainEvents);
    }
}
