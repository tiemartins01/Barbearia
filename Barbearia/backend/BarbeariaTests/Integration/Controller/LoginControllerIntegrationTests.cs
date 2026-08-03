using System.Net;
using System.Net.Http.Json;
using Barbearia.Core.DTO;
using Barbearia.Core.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BarbeariaTests.Integration.Controller;

[Collection(IntegrationCollection.Name)]
public sealed class LoginControllerIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly BarbeariaApiFactory _factory;
    private readonly HttpClient _client;

    public LoginControllerIntegrationTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
        _factory = new BarbeariaApiFactory(fixture.ConnectionString);
        _client = _factory.CreateClient(
            new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Usuarios.AddAsync(TestDataFactory.CriarCliente());
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task Post_Login_Valido_Deve_Retornar_204_E_Criar_Cookies()
    {
        var response = await _client.PostAsJsonAsync(
            "/login",
            new DTOLoginUsuario
            {
                Nome = "cliente",
                Senha = "123456"
            });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var cookies = response.Headers
            .GetValues("Set-Cookie")
            .ToArray();

        Assert.Contains(cookies, value => value.StartsWith("access-token="));
        Assert.Contains(cookies, value => value.StartsWith("refresh-token="));
        Assert.All(cookies, value => Assert.Contains("httponly", value.ToLowerInvariant()));
    }

    [Fact]
    public async Task Post_Login_Com_Senha_Invalida_Deve_Retornar_401()
    {
        var response = await _client.PostAsJsonAsync(
            "/login",
            new DTOLoginUsuario
            {
                Nome = "cliente",
                Senha = "errada"
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Login_Me_Sem_Cookie_Deve_Retornar_401()
    {
        using var clientSemCookies = _factory.CreateClient(
            new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = false
            });

        var response = await clientSemCookies.GetAsync("/login/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_Seguido_De_Me_Deve_Retornar_Usuario_Autenticado()
    {
        var login = await _client.PostAsJsonAsync(
            "/login",
            new DTOLoginUsuario
            {
                Nome = "cliente",
                Senha = "123456"
            });

        Assert.Equal(HttpStatusCode.NoContent, login.StatusCode);

        var response = await _client.GetAsync("/login/me");
        var body = await response.Content.ReadFromJsonAsync<DTOResponseMe>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Cliente Teste", body.Nome);
        Assert.Equal("Cliente", body.Role);
    }
}
