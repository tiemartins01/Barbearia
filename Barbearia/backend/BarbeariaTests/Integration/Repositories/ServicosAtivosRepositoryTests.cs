//using Barbearia.Core.Repository;

//namespace BarbeariaTests.Integration.Repositories;

//public sealed class ServicosAtivosRepositoryTests : IntegrationTestBase
//{
//    public ServicosAtivosRepositoryTests(PostgreSqlFixture fixture)
//        : base(fixture)
//    {
//    }

//    [Fact]
//    public async Task GetServicosAtivos_Deve_Retornar_Apenas_Ativos()
//    {
//        await using var context = Fixture.CreateContext();
//        await context.Servicos.AddRangeAsync(
//            TestDataFactory.CriarServico("Corte", ativo: true),
//            TestDataFactory.CriarServico("Barba", ativo: true),
//            TestDataFactory.CriarServico("Desativado", ativo: false));
//        await context.SaveChangesAsync();

//        var repository = new ServicosAtivosRepository(context);

//        var resultado = await repository.GetServicosAtivos();

//        Assert.Equal(2, resultado.Count);
//        Assert.DoesNotContain(resultado, x => x.NomeServico == "Desativado");
//    }

//    [Fact]
//    public async Task GetServicosAtivos_Deve_Ordenar_Por_Nome()
//    {
//        await using var context = Fixture.CreateContext();
//        await context.Servicos.AddRangeAsync(
//            TestDataFactory.CriarServico("Corte"),
//            TestDataFactory.CriarServico("Barba"));
//        await context.SaveChangesAsync();

//        var repository = new ServicosAtivosRepository(context);

//        var resultado = await repository.GetServicosAtivos();

//        Assert.Equal("Barba", resultado[0].NomeServico);
//        Assert.Equal("Corte", resultado[1].NomeServico);
//    }
//}
