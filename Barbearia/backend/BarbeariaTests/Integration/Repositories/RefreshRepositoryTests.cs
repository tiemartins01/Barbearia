//using Barbearia.Core.Repository;
//using Microsoft.EntityFrameworkCore;

//namespace BarbeariaTests.Integration.Repositories;

//public sealed class RefreshRepositoryTests : IntegrationTestBase
//{
//    public RefreshRepositoryTests(PostgreSqlFixture fixture)
//        : base(fixture)
//    {
//    }

//    [Fact]
//    public async Task SaveAsync_Deve_Persistir_Refresh_Token()
//    {
//        await using var context = Fixture.CreateContext();
//        var usuario = TestDataFactory.CriarCliente();
//        await context.Usuarios.AddAsync(usuario);
//        await context.SaveChangesAsync();

//        var repository = new RefreshRepository(context);
//        await repository.SaveAsync(
//            usuario.Id,
//            "refresh-token-1",
//            DateTime.Now.AddDays(7));
//        await context.SaveChangesAsync();

//        var salvo = await context.RefreshTokens
//            .AsNoTracking()
//            .SingleAsync();

//        Assert.Equal(usuario.Id, salvo.Id_usuario);
//        Assert.Equal("refresh-token-1", salvo.Token);
//        Assert.False(salvo.Revogado);
//    }

//    [Fact]
//    public async Task GetAsync_Deve_Retornar_Token_Existente()
//    {
//        await using var context = Fixture.CreateContext();
//        var usuario = TestDataFactory.CriarCliente();
//        await context.Usuarios.AddAsync(usuario);
//        await context.SaveChangesAsync();

//        var repository = new RefreshRepository(context);
//        await repository.SaveAsync(usuario.Id, "refresh-token-2", DateTime.Now.AddDays(7));
//        await context.SaveChangesAsync();
//        context.ChangeTracker.Clear();

//        var token = await repository.GetAsync("refresh-token-2");

//        Assert.NotNull(token);
//        Assert.Equal("refresh-token-2", token.Token);
//    }

//    [Fact]
//    public async Task RevokeAsync_Deve_Marcar_Token_Como_Revogado()
//    {
//        await using var context = Fixture.CreateContext();
//        var usuario = TestDataFactory.CriarCliente();
//        await context.Usuarios.AddAsync(usuario);
//        await context.SaveChangesAsync();

//        var repository = new RefreshRepository(context);
//        await repository.SaveAsync(usuario.Id, "refresh-token-3", DateTime.Now.AddDays(7));
//        await context.SaveChangesAsync();

//        await repository.RevokeAsync("refresh-token-3");
//        await context.SaveChangesAsync();

//        var salvo = await context.RefreshTokens
//            .AsNoTracking()
//            .SingleAsync();

//        Assert.True(salvo.Revogado);
//    }
//}
