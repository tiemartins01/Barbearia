//using Barbearia.Core.Repository;
//using Microsoft.EntityFrameworkCore;

//namespace BarbeariaTests.Integration.Repositories;

//public sealed class NovoClienteRepositoryTests : IntegrationTestBase
//{
//    public NovoClienteRepositoryTests(PostgreSqlFixture fixture)
//        : base(fixture)
//    {
//    }

//    [Fact]
//    public async Task CadastraNovoClienteAsync_Deve_Adicionar_Usuario()
//    {
//        await using var context = Fixture.CreateContext();
//        var repository = new NovoClienteRepository(context);
//        var usuario = TestDataFactory.CriarCliente();

//        await repository.CadastraNovoClienteAsync(usuario);
//        await context.SaveChangesAsync();

//        Assert.True(usuario.Id > 0);
//        Assert.Equal(1, await context.Usuarios.CountAsync());
//    }

//    [Theory]
//    [InlineData("cliente@teste.com", "11144477735", "11999998888", "outro")]
//    [InlineData("outro@teste.com", "52998224725", "11999998888", "outro")]
//    [InlineData("outro@teste.com", "11144477735", "11987654321", "outro")]
//    [InlineData("outro@teste.com", "11144477735", "11999998888", "cliente")]
//    public async Task VerificarDuplicidadeAsync_Deve_Detectar_Campo_Repetido(
//        string email,
//        string cpf,
//        string telefone,
//        string login)
//    {
//        await using var context = Fixture.CreateContext();
//        await context.Usuarios.AddAsync(TestDataFactory.CriarCliente());
//        await context.SaveChangesAsync();

//        var repository = new NovoClienteRepository(context);

//        var duplicado = await repository.VerificarDuplicidadeAsync(
//            email,
//            cpf,
//            telefone,
//            login);

//        Assert.NotNull(duplicado);
//    }

//    [Fact]
//    public async Task Banco_Deve_Rejeitar_Cpf_Duplicado()
//    {
//        await using var context = Fixture.CreateContext();
//        var repository = new NovoClienteRepository(context);

//        await repository.CadastraNovoClienteAsync(TestDataFactory.CriarCliente());
//        await context.SaveChangesAsync();

//        var duplicado = TestDataFactory.CriarCliente(
//            login: "outro",
//            email: "outro@teste.com",
//            cpf: "52998224725",
//            telefone: "11999998888");

//        await repository.CadastraNovoClienteAsync(duplicado);

//        await Assert.ThrowsAsync<DbUpdateException>(
//            () => context.SaveChangesAsync());
//    }
//}
