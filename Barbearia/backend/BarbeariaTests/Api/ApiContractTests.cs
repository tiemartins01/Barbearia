using BarbeariaApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BarbeariaTests.Api;

public sealed class ApiContractTests
{
    private static string[] Templates(Type controller)
        => controller.GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly)
            .SelectMany(m=>m.GetCustomAttributes().OfType<HttpMethodAttribute>())
            .Select(a=>a.Template).Where(x=>!string.IsNullOrWhiteSpace(x)).Select(x=>x!).ToArray();

    public static IEnumerable<object[]> VersionedRoutes()
    {
        yield return new object[]{typeof(LoginController),"~/api/v1/auth/login"};
        yield return new object[]{typeof(LoginController),"~/api/v1/auth/logout"};
        yield return new object[]{typeof(LoginController),"~/api/v1/auth/refresh"};
        yield return new object[]{typeof(LoginController),"~/api/v1/auth/me"};
        yield return new object[]{typeof(LoginController),"~/api/v1/auth/sessions"};
        yield return new object[]{typeof(LoginController),"~/api/v1/auth/sessions/{sessionId:int}"};
        yield return new object[]{typeof(NovoUsuarioController),"~/api/v1/users"};
        yield return new object[]{typeof(EnviarEmailController),"~/api/v1/password/recovery"};
        yield return new object[]{typeof(TrocarSenhaController),"~/api/v1/password/reset"};
        yield return new object[]{typeof(ServicosAtivosController),"~/api/v1/services"};
        yield return new object[]{typeof(ProximoAgendamentoController),"~/api/v1/appointments/next"};
        yield return new object[]{typeof(ProximoAgendamentoController),"~/api/v1/appointments/available-slots"};
        yield return new object[]{typeof(ProximoAgendamentoController),"~/api/v1/appointments"};
        yield return new object[]{typeof(AbaClienteController),"~/api/v1/barbers"};
        yield return new object[]{typeof(AbaClienteController),"~/api/v1/appointments/history"};
        yield return new object[]{typeof(AbaClienteController),"~/api/v1/users/me"};
        yield return new object[]{typeof(AbaClienteController),"~/api/v1/appointments/{idHorario:int}"};
        yield return new object[]{typeof(AbaClienteController),"~/api/v1/reviews"};
        yield return new object[]{typeof(CsrfController),"~/api/v1/security/csrf"};
    }

    [Theory]
    [MemberData(nameof(VersionedRoutes))]
    public void Endpoints_Publicados_Devem_Manter_Rotas_V1(Type controller,string route)
        => Assert.Contains(route,Templates(controller));

    [Fact]
    public void Controllers_De_Cliente_Devem_Exigir_Policy_ClientOnly()
    {
        foreach(var type in new[]{typeof(AbaClienteController),typeof(ProximoAgendamentoController),typeof(ServicosAtivosController)})
        {
            var auth=type.GetCustomAttributes<AuthorizeAttribute>().ToArray();
            Assert.Contains(auth,a=>a.Policy=="ClientOnly");
        }
    }

    [Fact]
    public void Csrf_Deve_Permitir_Anonimo()
    {
        var method=typeof(CsrfController).GetMethods().Single(m=>m.GetCustomAttributes<HttpGetAttribute>().Any());
        Assert.NotNull(method.GetCustomAttribute<AllowAnonymousAttribute>());
    }

    [Fact]
    public void Login_Me_E_Sessoes_Devem_Exigir_Autenticacao()
    {
        var methods=typeof(LoginController).GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly);
        foreach(var name in new[]{"Me","Sessions","RevokeAllSessions","RevokeSession"})
        {
            var candidates=methods.Where(m=>m.Name.Equals(name,StringComparison.OrdinalIgnoreCase)).ToArray();
            if(candidates.Length==0) continue;
            Assert.All(candidates,m=>Assert.NotNull(m.GetCustomAttribute<AuthorizeAttribute>()));
        }
    }

    [Fact]
    public void Logout_Deve_Permitir_Anonimo_Para_Ser_Idempotente()
    {
        var method=typeof(LoginController).GetMethods().First(m=>m.GetCustomAttributes<HttpPostAttribute>().Any(a=>a.Template=="logout"));
        Assert.NotNull(method.GetCustomAttribute<AllowAnonymousAttribute>());
    }

    [Fact]
    public void Api_Deve_Conter_Middlewares_De_Seguranca_Observabilidade_E_Performance()
    {
        var asm=typeof(Program).Assembly; var names=asm.GetTypes().Select(t=>t.Name).ToHashSet();
        foreach(var x in new[]{"ErrorHandlingMiddleware","PerformanceMonitoringMiddleware","RequestLoggingMiddleware","SecurityEventLoggingMiddleware","SecurityHeadersMiddleware"}) Assert.Contains(x,names);
    }

    [Fact]
    public void Api_Deve_Conter_HealthCheck_De_Banco_E_Metricas()
    {
        var names=typeof(Program).Assembly.GetTypes().Select(t=>t.Name).ToHashSet();
        Assert.Contains("DatabaseHealthCheck",names); Assert.Contains("ApiMetrics",names);
    }
}
