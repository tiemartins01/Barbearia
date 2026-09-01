using BarbeariaCore.Domain.Common;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.ValueObjects;

namespace BarbeariaTests.Architecture;

public sealed class ArchitectureTests
{
    private static Assembly Core => typeof(AggregateRoot).Assembly;

    [Fact]
    public void Domain_Nao_Deve_Referenciar_Infrastructure_Api_EF_JWT_BCrypt()
    {
        var forbidden = new[]{"BarbeariaInfrastructure","BarbeariaApi","Microsoft.EntityFrameworkCore","Microsoft.IdentityModel","System.IdentityModel.Tokens.Jwt","BCrypt"};
        var refs=Core.GetReferencedAssemblies().Select(x=>x.Name!).ToArray();
        foreach(var f in forbidden) Assert.DoesNotContain(refs,r=>r.StartsWith(f,StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Entidades_De_Dominio_Nao_Devem_Ter_Setters_Publicos()
    {
        var types=new[]{typeof(Usuario),typeof(Agendamento),typeof(Servico),typeof(Barbeiro),typeof(Avaliacao)};
        var offenders=types.SelectMany(t=>t.GetProperties().Where(p=>p.SetMethod?.IsPublic==true).Select(p=>$"{t.Name}.{p.Name}")).ToArray();
        Assert.Empty(offenders);
    }

    [Fact]
    public void ValueObjects_Devem_Ser_Sealed()
    {
        Assert.True(typeof(Email).IsSealed); Assert.True(typeof(Cpf).IsSealed); Assert.True(typeof(Telefone).IsSealed); Assert.True(typeof(Senha).IsSealed);
    }

    [Fact]
    public void Senha_Nao_Deve_Referenciar_IPasswordHash()
    {
        var deps=typeof(Senha).GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.Static)
            .SelectMany(m=>m.GetParameters()).Select(p=>p.ParameterType.Name).ToArray();
        Assert.DoesNotContain("IPasswordHash",deps);
    }

    [Fact]
    public void Nomes_Legados_De_Dominio_Nao_Devem_Existir()
    {
        var forbidden=new[]{"Horarios","Servicos","Avaliacoes","Phone"};
        var domainTypes=Core.GetTypes().Where(t=>t.Namespace?.Contains("Domain") == true).Select(t=>t.Name).ToHashSet();
        foreach(var x in forbidden) Assert.DoesNotContain(x,domainTypes);
    }

    [Fact]
    public void Nomes_Oficiais_De_Dominio_Devem_Existir()
    {
        var required=new[]{"Usuario","Agendamento","Servico","Barbeiro","Avaliacao","Email","Cpf","Telefone","Senha"};
        var names=Core.GetTypes().Select(t=>t.Name).ToHashSet(); foreach(var x in required) Assert.Contains(x,names);
    }

    [Fact]
    public void AggregateRoot_DomainEvents_Deve_Ser_Somente_Leitura()
    {
        var p=typeof(AggregateRoot).GetProperty("DomainEvents")!;
        Assert.False(p.CanWrite);
        Assert.True(typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType));
    }

    [Fact]
    public void Domain_Nao_Deve_Expor_DateTimeNow_Em_Overloads_Temporais_Do_Usuario()
    {
        var temporal=new[]{"PodeAutenticar","RegistrarFalhaLogin","GerarCodigo","PodeTrocarSenha","CodigoIsValido"};
        foreach(var name in temporal)
        {
            var methods=typeof(Usuario).GetMethods(BindingFlags.Public|BindingFlags.Instance).Where(m=>m.Name==name).ToArray();
            Assert.NotEmpty(methods);
            Assert.All(methods,m=>Assert.Contains(m.GetParameters(),p=>p.ParameterType==typeof(DateTime)));
        }
    }

    [Fact]
    public void RefreshToken_E_AuditLog_Nao_Devem_Estar_No_Domain()
    {
        var domainNames=Core.GetTypes().Where(t=>t.Namespace?.Contains("Domain") == true).Select(t=>t.Name).ToArray();
        Assert.DoesNotContain("RefreshToken",domainNames); Assert.DoesNotContain("AuditLog",domainNames);
    }
}
