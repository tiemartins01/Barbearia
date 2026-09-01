using BarbeariaCore.Domain.Common;

namespace BarbeariaTests.Application;

public sealed class ApplicationStructureTests
{
    private static Assembly Core => typeof(AggregateRoot).Assembly;

    public static IEnumerable<object[]> RequiredUseCases()
    {
        string[] names = [
            "RealizarLogin","CadastrarCliente","SolicitarRecuperacaoSenha","RedefinirSenha",
            "CriarAgendamento","ConsultarProximoAgendamento","ConsultarHorariosDisponiveis",
            "AlterarDadosPessoais","AvaliarAtendimento","ConsultarAgendamento",
            "ConsultarAgendamentoDoCliente","ConsultarDadosPessoais","ConsultarHistoricoCliente",
            "ListarBarbeiros","ListarServicosAtivos","RenovarToken","RevogarToken",
            "ListarSessoes","RevogarTodasSessoes","RevogarSessao"
        ];
        return names.Select(x => new object[] { x });
    }

    [Theory]
    [MemberData(nameof(RequiredUseCases))]
    public void Todo_Caso_De_Uso_Atual_Deve_Existir(string name)
    {
        var type = Core.GetTypes().SingleOrDefault(t => t.Name == name && t.Namespace?.Contains("UseCases") == true);
        Assert.NotNull(type);
    }

    public static IEnumerable<object[]> LegacyServices()
    {
        string[] names = ["AbaClienteService","ProximoAtendimentoService","LoginService","NovoClienteService","EmailEsqueciSenhaService","TrocaSenhaService","ServicosAtivosService"];
        return names.Select(x => new object[] { x });
    }

    [Theory]
    [MemberData(nameof(LegacyServices))]
    public void Services_Legados_Nao_Devem_Voltar(string name)
        => Assert.DoesNotContain(Core.GetTypes(), t => t.Name == name && t.Namespace?.Contains("Application", StringComparison.OrdinalIgnoreCase) == true);

    [Fact]
    public void Application_Nao_Deve_Depender_Diretamente_De_EFCore()
    {
        var violations = Core.GetTypes()
            .Where(t => t.Namespace?.Contains("UseCases") == true)
            .SelectMany(t => t.GetConstructors())
            .SelectMany(c => c.GetParameters().Select(p => (Owner:c.DeclaringType!, Dep:p.ParameterType)))
            .Where(x => x.Dep.Namespace?.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal) == true)
            .Select(x => $"{x.Owner.Name}->{x.Dep.Name}")
            .ToArray();
        Assert.Empty(violations);
    }

    [Fact]
    public void Casos_De_Uso_Devem_Ser_Sealed()
    {
        var offenders = Core.GetTypes()
            .Where(t => t.IsClass && t.Namespace?.Contains("UseCases") == true && !t.IsAbstract && !t.IsSealed)
            .Select(t => t.FullName)
            .ToArray();
        Assert.Empty(offenders);
    }

    [Fact]
    public void Repositories_Por_Aggregate_Devem_Existir()
    {
        var required = new[] {"IUsuarioRepository","IAgendamentoRepository","IBarbeiroRepository","IServicoRepository","IAvaliacaoRepository"};
        var names = Core.GetTypes().Select(t => t.Name).ToHashSet();
        foreach (var item in required) Assert.Contains(item,names);
    }

    [Fact]
    public void Queries_De_Leitura_Devem_Existir()
    {
        var required = new[] {"IAgendaDisponibilidadeQuery","IBarbeirosQuery","IDadosPessoaisQuery","IHistoricoClienteQuery","IProximoAgendamentoQuery","IServicosAtivosQuery"};
        var names = Core.GetTypes().Select(t => t.Name).ToHashSet();
        foreach (var item in required) Assert.Contains(item,names);
    }

    [Fact]
    public void AggregateRepositories_Nao_Devem_Retornar_DTOs()
    {
        var repoTypes = Core.GetTypes().Where(t => t.IsInterface && t.Name.EndsWith("Repository") && t.Namespace?.Contains("Repositories") == true);
        var offenders = repoTypes.SelectMany(t => t.GetMethods().Select(m => (Type:t,Method:m)))
            .Where(x => x.Method.ReturnType.ToString().Contains("DTO",StringComparison.OrdinalIgnoreCase))
            .Select(x => $"{x.Type.Name}.{x.Method.Name}").ToArray();
        Assert.Empty(offenders);
    }

    [Fact]
    public void UseCases_Async_Devem_Receber_CancellationToken_Contrato_Alvo()
    {
        var offenders = Core.GetTypes().Where(t => t.IsClass && t.Namespace?.Contains("UseCases") == true)
            .SelectMany(t => t.GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly))
            .Where(m => typeof(Task).IsAssignableFrom(m.ReturnType) && !m.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken)))
            .Select(m => $"{m.DeclaringType!.Name}.{m.Name}").ToArray();

        // Este contrato expõe uma lacuna real atual: os casos de uso assíncronos ainda não propagam CancellationToken.
        Assert.Empty(offenders);
    }
}
