using System.Net;
using System.Net.Http.Json;
using Barbearia.Core.DTO;
using Barbearia.Core.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BarbeariaTests.Integration.Authentication;

[Collection(IntegrationCollection.Name)]
public sealed class JwtAuthenticationIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly Controller.BarbeariaApiFactory _factory;
    private readonly HttpClient _client;

    public JwtAuthenticationIntegrationTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
        _factory = new Controller.BarbeariaApiFactory(fixture.ConnectionString);
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
    public async Task Jwt_No_Cookie_Deve_Autorizar_Endpoint_Me()
    {
        var login = await _client.PostAsJsonAsync(
            "/login",
            new DTOLoginUsuario
            {
                Nome = "cliente",
                Senha = "123456"
            });

        Assert.Equal(HttpStatusCode.NoContent, login.StatusCode);

        var me = await _client.GetAsync("/login/me");

        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
    }

    [Fact]
    public async Task Cookie_Jwt_Invalido_Deve_Retornar_401()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/login/me");
        request.Headers.Add("Cookie", "access-token=token-invalido");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
