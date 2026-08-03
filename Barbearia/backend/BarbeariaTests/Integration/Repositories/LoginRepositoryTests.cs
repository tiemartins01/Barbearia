using Barbearia.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaTests.Integration.Repositories;

public sealed class LoginRepositoryTests : IntegrationTestBase
{
    public LoginRepositoryTests(PostgreSqlFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ObterPorLoginAsync_Deve_Retornar_Usuario_Existente()
    {
        await using var context = Fixture.CreateContext();
        await context.Usuarios.AddAsync(TestDataFactory.CriarCliente());
        await context.SaveChangesAsync();

        var repository = new LoginRepository(context);

        var usuario = await repository.ObterPorLoginAsync("cliente");

        Assert.NotNull(usuario);
        Assert.Equal("cliente", usuario.Login);
    }

    [Fact]
    public async Task ObterPorLoginAsync_Deve_Retornar_Nulo_Quando_Nao_Existir()
    {
        await using var context = Fixture.CreateContext();
        var repository = new LoginRepository(context);

        var usuario = await repository.ObterPorLoginAsync("inexistente");

        Assert.Null(usuario);
    }

    [Fact]
    public async Task Atualizar_Deve_Persistir_Tentativa_De_Login()
    {
        await using var context = Fixture.CreateContext();
        await context.Usuarios.AddAsync(TestDataFactory.CriarCliente());
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = new LoginRepository(context);
        var usuario = await repository.ObterPorLoginAsync("cliente");

        Assert.NotNull(usuario);
        usuario.RegistrarFalhaLogin();

        await repository.Atualizar(usuario);
        await context.SaveChangesAsync();

        await using var leitura = Fixture.CreateContext();
        var salvo = await leitura.Usuarios
            .AsNoTracking()
            .SingleAsync(x => x.Login == "cliente");

        Assert.Equal(1, salvo.TentativasLogin);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Entidade_Sem_Tracking()
    {
        await using var context = Fixture.CreateContext();
        var usuario = TestDataFactory.CriarCliente();
        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = new LoginRepository(context);

        var encontrado = await repository.ObterPorIdAsync(usuario.Id);

        Assert.NotNull(encontrado);
        Assert.Equal(
            Microsoft.EntityFrameworkCore.EntityState.Detached,
            context.Entry(encontrado).State);
    }
}
