using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaCore.UseCases.Autenticacao;
using BarbeariaTests.Helpers;
using Microsoft.Extensions.Logging;

namespace BarbeariaTests.Application;

public sealed class AutenticacaoUseCasesTests
{
    private static Usuario UsuarioValido(bool ativado = true)
    {
        var u = new Usuario("Maria",new Email("maria@exemplo.com"),new Telefone("11999999999"),new Cpf("52998224725"),"maria",Senha.DeHash("hash-atual"),RolePerson.Cliente,ativado,null);
        ReflectionHelper.SetPrivateProperty(u,"Id",10);
        return u;
    }

    [Fact]
    public async Task RealizarLogin_Usuario_Inexistente_Deve_Falhar_Sem_Revelar_Motivo()
    {
        var usuarios=new Mock<IUsuarioRepository>();
        usuarios.Setup(x=>x.ObterPorLoginAsync("maria")).ReturnsAsync((Usuario?)null);
        var sut=new RealizarLogin(usuarios.Object,Mock.Of<ITokenService>(),Mock.Of<IRefreshRepository>(),Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<RealizarLogin>>(),Mock.Of<IPasswordHash>());
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync(" MARIA ","senha"));
        Assert.Equal("AUTH_INVALID_CREDENTIALS",ex.Code);
    }

    [Fact]
    public async Task RealizarLogin_Usuario_Bloqueado_Deve_Falhar_Sem_Verificar_Senha()
    {
        var u=UsuarioValido(); for(int i=0;i<5;i++)u.RegistrarFalhaLogin(DateTime.Now);
        var usuarios=new Mock<IUsuarioRepository>(); usuarios.Setup(x=>x.ObterPorLoginAsync("maria")).ReturnsAsync(u);
        var hash=new Mock<IPasswordHash>();
        var sut=new RealizarLogin(usuarios.Object,Mock.Of<ITokenService>(),Mock.Of<IRefreshRepository>(),Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<RealizarLogin>>(),hash.Object);
        await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("maria","senha"));
        hash.Verify(x=>x.Verify(It.IsAny<string>(),It.IsAny<string>()),Times.Never);
    }

    [Fact]
    public async Task RealizarLogin_Senha_Incorreta_Deve_Registrar_Falha_E_Salvar()
    {
        var u=UsuarioValido(); var usuarios=new Mock<IUsuarioRepository>(); usuarios.Setup(x=>x.ObterPorLoginAsync("maria")).ReturnsAsync(u);
        var hash=new Mock<IPasswordHash>(); hash.Setup(x=>x.Verify("errada","hash-atual")).Returns(false);
        var uow=new Mock<IUnitOfWork>();
        var sut=new RealizarLogin(usuarios.Object,Mock.Of<ITokenService>(),Mock.Of<IRefreshRepository>(),uow.Object,Mock.Of<ILogger<RealizarLogin>>(),hash.Object);
        await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("maria","errada"));
        Assert.Equal(1,u.TentativasLogin);
        usuarios.Verify(x=>x.AtualizarAsync(u),Times.Once);
        uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Fact]
    public async Task RealizarLogin_Valido_Deve_Resetar_Tentativas_Gerar_Tokens_E_Persistir_Refresh()
    {
        var u=UsuarioValido(); u.RegistrarFalhaLogin(DateTime.Now);
        var usuarios=new Mock<IUsuarioRepository>(); usuarios.Setup(x=>x.ObterPorLoginAsync("maria")).ReturnsAsync(u);
        var hash=new Mock<IPasswordHash>(); hash.Setup(x=>x.Verify("senha","hash-atual")).Returns(true);
        var token=new Mock<ITokenService>(); token.Setup(x=>x.GenerateToken(u)).Returns("access"); token.Setup(x=>x.GenerateRefreshToken()).Returns("refresh");
        var refresh=new Mock<IRefreshRepository>(); var uow=new Mock<IUnitOfWork>();
        var sut=new RealizarLogin(usuarios.Object,token.Object,refresh.Object,uow.Object,Mock.Of<ILogger<RealizarLogin>>(),hash.Object);
        var result=await sut.ExecutarAsync(" MARIA ","senha");
        Assert.Equal("access",result.accessToken); Assert.Equal("refresh",result.refreshToken); Assert.Equal(0,u.TentativasLogin);
        refresh.Verify(x=>x.SaveAsync(10,"refresh",It.Is<DateTime>(d=>d>DateTime.UtcNow.AddDays(6))),Times.Once);
        uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Theory]
    [InlineData("email")]
    [InlineData("cpf")]
    [InlineData("telefone")]
    [InlineData("login")]
    public async Task CadastrarCliente_Duplicidade_Deve_Falhar(string campo)
    {
        var usuarios=new Mock<IUsuarioRepository>(); var existente=UsuarioValido();
        if(campo=="email")usuarios.Setup(x=>x.ObterPorEmailAsync("maria@exemplo.com")).ReturnsAsync(existente);
        if(campo=="cpf")usuarios.Setup(x=>x.ObterPorCpfAsync("52998224725")).ReturnsAsync(existente);
        if(campo=="telefone")usuarios.Setup(x=>x.ObterPorTelefoneAsync("11999999999")).ReturnsAsync(existente);
        if(campo=="login")usuarios.Setup(x=>x.ObterPorLoginAsync("maria")).ReturnsAsync(existente);
        var sut=new CadastrarCliente(usuarios.Object,Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<CadastrarCliente>>(),Mock.Of<IPasswordHash>());
        await Assert.ThrowsAsync<DomainException>(()=>sut.ExecutarAsync("Maria","maria@exemplo.com","11999999999","52998224725","maria","123456","") );
    }

    [Fact]
    public async Task CadastrarCliente_Valido_Deve_Hashear_Adicionar_Salvar_Registrar_Evento_E_Salvar_Novamente()
    {
        var usuarios=new Mock<IUsuarioRepository>();
        usuarios.Setup(x=>x.AdicionarAsync(It.IsAny<Usuario>())).Callback<Usuario>(u=>ReflectionHelper.SetPrivateProperty(u,"Id",123)).Returns(Task.CompletedTask);
        var hash=new Mock<IPasswordHash>(); hash.Setup(x=>x.Hash("123456")).Returns("hash-novo"); var uow=new Mock<IUnitOfWork>();
        var sut=new CadastrarCliente(usuarios.Object,uow.Object,Mock.Of<ILogger<CadastrarCliente>>(),hash.Object);
        var result=await sut.ExecutarAsync(" Maria ","maria@exemplo.com","11999999999","52998224725"," MARIA ","123456","");
        Assert.True(result.Sucesso);
        hash.Verify(x=>x.Hash("123456"),Times.Once);
        usuarios.Verify(x=>x.AdicionarAsync(It.Is<Usuario>(u=>u.Login=="maria" && u.Senha.Hash=="hash-novo")),Times.Once);
        uow.Verify(x=>x.SaveChangesAsync(),Times.Exactly(2));
    }

    [Fact]
    public async Task SolicitarRecuperacao_Email_Inexistente_Deve_Retornar_Sem_Enviar_Email()
    {
        var usuarios=new Mock<IUsuarioRepository>(); usuarios.Setup(x=>x.ObterPorEmailAsync("x@x.com")).ReturnsAsync((Usuario?)null);
        var email=new Mock<IEnviarEmail>(); var sut=new SolicitarRecuperacaoSenha(usuarios.Object,email.Object,Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<SolicitarRecuperacaoSenha>>());
        await sut.ExecutarAsync(" X@X.COM ");
        email.Verify(x=>x.EnviarEmailAsync(It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>()),Times.Never);
    }

    [Fact]
    public async Task SolicitarRecuperacao_Usuario_Existente_Deve_Gerar_Codigo_Salvar_E_Enviar_Email()
    {
        var u=UsuarioValido(); var usuarios=new Mock<IUsuarioRepository>(); usuarios.Setup(x=>x.ObterPorEmailAsync("maria@exemplo.com")).ReturnsAsync(u);
        var email=new Mock<IEnviarEmail>(); var uow=new Mock<IUnitOfWork>(); var sut=new SolicitarRecuperacaoSenha(usuarios.Object,email.Object,uow.Object,Mock.Of<ILogger<SolicitarRecuperacaoSenha>>());
        await sut.ExecutarAsync(" MARIA@EXEMPLO.COM ");
        Assert.True(u.CodigoAtivo); Assert.Equal(6,u.Codigo!.Length);
        usuarios.Verify(x=>x.AtualizarAsync(u),Times.Once); uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
        email.Verify(x=>x.EnviarEmailAsync("maria@exemplo.com","Troca de senha",It.Is<string>(m=>m.Contains(u.Codigo!))),Times.Once);
    }

    [Fact]
    public async Task RedefinirSenha_Usuario_Inexistente_Ou_Inativo_Deve_Falhar()
    {
        var usuarios=new Mock<IUsuarioRepository>();
        var sut=new RedefinirSenha(usuarios.Object,Mock.Of<IUnitOfWork>(),Mock.Of<IPasswordHash>());
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("123456","x@x.com","abcdef","abcdef"));
        Assert.Equal("PASSWORD_RESET_INVALID_DATA",ex.Code);
    }

    [Fact]
    public async Task RedefinirSenha_Senhas_Diferentes_Deve_Falhar()
    {
        var u=UsuarioValido(); var usuarios=new Mock<IUsuarioRepository>(); usuarios.Setup(x=>x.ObterPorEmailAsync("maria@exemplo.com")).ReturnsAsync(u);
        var sut=new RedefinirSenha(usuarios.Object,Mock.Of<IUnitOfWork>(),Mock.Of<IPasswordHash>());
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ValidationException>(()=>sut.ExecutarAsync("123456","maria@exemplo.com","abcdef","abcdefg"));
        Assert.Equal("PASSWORD_RESET_PASSWORD_MISMATCH",ex.Code);
    }

    [Fact]
    public async Task RedefinirSenha_Codigo_Expirado_Deve_Falhar()
    {
        var u=UsuarioValido(); u.GerarCodigo("123456",DateTime.Now.AddMinutes(-20));
        var usuarios=new Mock<IUsuarioRepository>(); usuarios.Setup(x=>x.ObterPorEmailAsync("maria@exemplo.com")).ReturnsAsync(u);
        var sut=new RedefinirSenha(usuarios.Object,Mock.Of<IUnitOfWork>(),Mock.Of<IPasswordHash>());
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("123456","maria@exemplo.com","abcdef","abcdef"));
        Assert.Equal("PASSWORD_RESET_CODE_EXPIRED",ex.Code);
    }

    [Fact]
    public async Task RedefinirSenha_Codigo_Incorreto_Deve_Incrementar_Tentativa_E_Salvar()
    {
        var u=UsuarioValido(); u.GerarCodigo("123456",DateTime.Now);
        var usuarios=new Mock<IUsuarioRepository>(); usuarios.Setup(x=>x.ObterPorEmailAsync("maria@exemplo.com")).ReturnsAsync(u); var uow=new Mock<IUnitOfWork>();
        var sut=new RedefinirSenha(usuarios.Object,uow.Object,Mock.Of<IPasswordHash>());
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync("000000","maria@exemplo.com","abcdef","abcdef"));
        Assert.Equal("PASSWORD_RESET_INVALID_CODE",ex.Code); Assert.Equal(1,u.TentativasCodigo);
        uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Fact]
    public async Task RedefinirSenha_Valida_Deve_Hashear_Limpar_Codigo_E_Salvar()
    {
        var u=UsuarioValido(); u.GerarCodigo("123456",DateTime.Now);
        var usuarios=new Mock<IUsuarioRepository>(); usuarios.Setup(x=>x.ObterPorEmailAsync("maria@exemplo.com")).ReturnsAsync(u); var uow=new Mock<IUnitOfWork>(); var hash=new Mock<IPasswordHash>(); hash.Setup(x=>x.Hash("abcdef")).Returns("novo-hash");
        var sut=new RedefinirSenha(usuarios.Object,uow.Object,hash.Object);
        var result=await sut.ExecutarAsync("123456"," MARIA@EXEMPLO.COM ","abcdef","abcdef");
        Assert.True(result.Sucesso); Assert.Equal("novo-hash",u.Senha.Hash); Assert.False(u.CodigoAtivo);
        usuarios.Verify(x=>x.AtualizarAsync(u),Times.Once); uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }
}
