using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Application.Models;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaCore.UseCases.Security;
using BarbeariaCore.UseCases.Servicos;
using BarbeariaTests.Helpers;

namespace BarbeariaTests.Application;

public sealed class SecurityAndServiceUseCasesTests
{
    private static Usuario UsuarioValido(bool ativo=true)
    {
        var u=new Usuario("Maria",new Email("maria@exemplo.com"),new Telefone("11999999999"),new Cpf("52998224725"),"maria",Senha.DeHash("hash"),RolePerson.Cliente,ativo,null);
        ReflectionHelper.SetPrivateProperty(u,"Id",1); return u;
    }

    [Fact]
    public async Task ListarSessoes_Deve_Mapear_E_Marcar_Token_Atual()
    {
        var repo=new Mock<IRefreshRepository>(); repo.Setup(x=>x.ListByUserAsync(1)).ReturnsAsync(new[]{new RefreshTokenData{Id=10,Token="atual",CriadoEm=DateTime.UtcNow,ExpiraEm=DateTime.UtcNow.AddDays(1)},new RefreshTokenData{Id=11,Token="outro",CriadoEm=DateTime.UtcNow,ExpiraEm=DateTime.UtcNow.AddDays(1)}});
        var result=await new ListarSessoes(repo.Object).ExecutarAsync(1,"atual");
        Assert.Equal(2,result.Count); Assert.True(result.Single(x=>x.Id==10).Atual); Assert.False(result.Single(x=>x.Id==11).Atual);
    }

    [Fact]
    public async Task RenovarToken_Inexistente_Deve_Falhar()
    {
        var repo=new Mock<IRefreshRepository>(); var sut=new RenovarToken(repo.Object,Mock.Of<ITokenService>(),Mock.Of<IUsuarioRepository>(),Mock.Of<IUnitOfWork>());
        Assert.Equal("INVALID_REFRESH",(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("x"))).Code);
    }

    [Fact]
    public async Task RenovarToken_Revogado_Com_Substituto_Deve_Revogar_Familia_E_Falhar()
    {
        var family=Guid.NewGuid(); var repo=new Mock<IRefreshRepository>(); repo.Setup(x=>x.GetAsync("x")).ReturnsAsync(new RefreshTokenData{Token="x",Revogado=true,ReplacedByToken="y",FamilyId=family,ExpiraEm=DateTime.UtcNow.AddDays(1)}); var uow=new Mock<IUnitOfWork>();
        var sut=new RenovarToken(repo.Object,Mock.Of<ITokenService>(),Mock.Of<IUsuarioRepository>(),uow.Object);
        await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("x")); repo.Verify(x=>x.RevokeFamilyAsync(family,"REFRESH_TOKEN_REUSE_DETECTED"),Times.Once); uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Fact]
    public async Task RenovarToken_Expirado_Deve_Falhar()
    {
        var repo=new Mock<IRefreshRepository>(); repo.Setup(x=>x.GetAsync("x")).ReturnsAsync(new RefreshTokenData{Token="x",ExpiraEm=DateTime.UtcNow.AddMinutes(-1)});
        var sut=new RenovarToken(repo.Object,Mock.Of<ITokenService>(),Mock.Of<IUsuarioRepository>(),Mock.Of<IUnitOfWork>()); Assert.Equal("INVALID_REFRESH",(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("x"))).Code);
    }

    [Fact]
    public async Task RenovarToken_Usuario_Inexistente_Ou_Inativo_Deve_Falhar()
    {
        var repo=new Mock<IRefreshRepository>(); repo.Setup(x=>x.GetAsync("x")).ReturnsAsync(new RefreshTokenData{Token="x",UsuarioId=1,ExpiraEm=DateTime.UtcNow.AddDays(1)}); var users=new Mock<IUsuarioRepository>();
        var sut=new RenovarToken(repo.Object,Mock.Of<ITokenService>(),users.Object,Mock.Of<IUnitOfWork>()); Assert.Equal("AUTH_INVALID_CREDENTIALS",(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("x"))).Code);
    }

    [Fact]
    public async Task RenovarToken_Valido_Deve_Rotacionar_Token_Manter_Familia_E_Salvar()
    {
        var family=Guid.NewGuid(); var repo=new Mock<IRefreshRepository>(); repo.Setup(x=>x.GetAsync("old")).ReturnsAsync(new RefreshTokenData{Token="old",UsuarioId=1,FamilyId=family,ExpiraEm=DateTime.UtcNow.AddDays(1)}); var users=new Mock<IUsuarioRepository>(); var u=UsuarioValido(); users.Setup(x=>x.ObterPorIdAsync(1)).ReturnsAsync(u); var token=new Mock<ITokenService>(); token.Setup(x=>x.GenerateToken(u)).Returns("access"); token.Setup(x=>x.GenerateRefreshToken()).Returns("new"); var uow=new Mock<IUnitOfWork>();
        var result=await new RenovarToken(repo.Object,token.Object,users.Object,uow.Object).ExecutarAsync("old"); Assert.Equal("access",result.accessToken); Assert.Equal("new",result.refreshToken); repo.Verify(x=>x.RevokeAsync("old","new","ROTATED"),Times.Once); repo.Verify(x=>x.SaveAsync(1,"new",It.IsAny<DateTime>(),family,null),Times.Once); uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Fact]
    public async Task RevogarToken_Inexistente_Ou_Ja_Revogado_Deve_Falhar()
    {
        var repo=new Mock<IRefreshRepository>(); var sut=new RevogarToken(repo.Object,Mock.Of<IUnitOfWork>()); Assert.Equal("INVALID_REFRESH",(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("x"))).Code);
    }

    [Fact]
    public async Task RevogarToken_Valido_Deve_Revogar_E_Salvar()
    {
        var repo=new Mock<IRefreshRepository>(); repo.Setup(x=>x.GetAsync("x")).ReturnsAsync(new RefreshTokenData{Token="x",ExpiraEm=DateTime.UtcNow.AddDays(1)}); var uow=new Mock<IUnitOfWork>(); await new RevogarToken(repo.Object,uow.Object).ExecutarAsync("x"); repo.Verify(x=>x.RevokeAsync("x",null,"LOGOUT"),Times.Once); uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Fact]
    public async Task RevogarTodasSessoes_Deve_Revogar_E_Salvar()
    {
        var repo=new Mock<IRefreshRepository>(); var uow=new Mock<IUnitOfWork>(); await new RevogarTodasSessoes(repo.Object,uow.Object).ExecutarAsync(1); repo.Verify(x=>x.RevokeAllByUserAsync(1),Times.Once); uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Fact]
    public async Task RevogarSessao_Inexistente_Deve_Falhar_Sem_Salvar()
    {
        var repo=new Mock<IRefreshRepository>(); repo.Setup(x=>x.RevokeByIdAsync(10,1)).ReturnsAsync(false); var uow=new Mock<IUnitOfWork>(); var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.NotFoundException>(()=>new RevogarSessao(repo.Object,uow.Object).ExecutarAsync(1,10)); Assert.Equal("SESSION_NOT_FOUND",ex.Code); uow.Verify(x=>x.SaveChangesAsync(),Times.Never);
    }

    [Fact]
    public async Task RevogarSessao_Existente_Deve_Salvar()
    {
        var repo=new Mock<IRefreshRepository>(); repo.Setup(x=>x.RevokeByIdAsync(10,1)).ReturnsAsync(true); var uow=new Mock<IUnitOfWork>(); await new RevogarSessao(repo.Object,uow.Object).ExecutarAsync(1,10); uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Fact]
    public async Task ListarServicosAtivos_Deve_Delegar_Para_Query()
    {
        var q=new Mock<IServicosAtivosQuery>(); q.Setup(x=>x.ListarAsync()).ReturnsAsync(new[]{new DTOServicosAtivos{Id=1,NomeServico="Corte",Preco=50,Duracao=30}}); var result=await new ListarServicosAtivos(q.Object).ExecutarAsync(); Assert.Single(result); Assert.Equal("Corte",result[0].NomeServico);
    }
}
