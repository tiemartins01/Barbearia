using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaCore.Security;
using BarbeariaTests.Helpers;
using System.Reflection.Emit;
namespace BarbeariaTests.Domain;

public sealed class SecurityAndCommonTests
{
    [Fact]
    public void CodeGenerator_Deve_Gerar_Exatamente_6_Digitos()
    {
        var generator = new CodigoRecuperacaoGenerator();
        for (var i=0;i<100;i++)
        {
            var code = generator.Gerar();
            Assert.Equal(6, code.Length);
            Assert.True(code.All(char.IsDigit));
        }
    }

    [Fact]
    public void ClearDomainEvents_Deve_Remover_Todos_Eventos()
    {
        var agora = new DateTime(
    2026, 9, 2,
    15, 0, 0,
    DateTimeKind.Utc);

        var horario = agora.AddHours(1);
        var u = new Usuario("Nome",new Email("a@b.com"),new Telefone("11999999999"),new Cpf("52998224725"),"login",Senha.DeHash("hash"),RolePerson.Cliente,true,null);
        ReflectionHelper.SetPrivateProperty(u,"Id",1);
        u.RegistrarCriacao(horario);
        u.AlterarSenhaPerfil(Senha.DeHash("novo"), horario);
        Assert.Equal(2,u.DomainEvents.Count);
        u.ClearDomainEvents();
        Assert.Empty(u.DomainEvents);
    }
}
