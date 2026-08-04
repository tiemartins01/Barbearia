//using Barbearia.Core.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;

//namespace BarbeariaTests.Integration.Database;

//public sealed class AppDbContextIntegrationTests : IntegrationTestBase
//{
//    public AppDbContextIntegrationTests(PostgreSqlFixture fixture)
//        : base(fixture)
//    {
//    }

//    [Fact]
//    public async Task Deve_Conectar_Ao_PostgreSql_E_Executar_Consulta()
//    {
//        await using var context = Fixture.CreateContext();

//        var podeConectar = await context.Database.CanConnectAsync();

//        Assert.True(podeConectar);
//    }

//    [Fact]
//    public async Task Deve_Persistir_E_Ler_Usuario()
//    {
//        await using var context = Fixture.CreateContext();
//        var usuario = TestDataFactory.CriarCliente();

//        await context.Usuarios.AddAsync(usuario);
//        await context.SaveChangesAsync();

//        await using var leitura = Fixture.CreateContext();
//        var salvo = await leitura.Usuarios
//            .AsNoTracking()
//            .SingleAsync(x => x.Login == "cliente");

//        Assert.True(salvo.Id > 0);
//        Assert.Equal("Cliente Teste", salvo.Nome);
//        Assert.Equal("cliente@teste.com", salvo.Email.EmailPessoa);
//    }

//    [Fact]
//    public async Task Banco_Deve_Rejeitar_Login_Duplicado()
//    {
//        await using var context = Fixture.CreateContext();

//        var primeiro = TestDataFactory.CriarCliente();
//        var segundo = TestDataFactory.CriarCliente(
//            login: "cliente",
//            email: "outro@teste.com",
//            cpf: "11144477735",
//            telefone: "11999998888");

//        await context.Usuarios.AddAsync(primeiro);
//        await context.SaveChangesAsync();

//        await context.Usuarios.AddAsync(segundo);

//        await Assert.ThrowsAsync<DbUpdateException>(
//            () => context.SaveChangesAsync());
//    }
//}
