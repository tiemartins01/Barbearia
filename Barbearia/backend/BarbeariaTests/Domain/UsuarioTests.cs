using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.Events;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaTests.Helpers;

namespace BarbeariaTests.Domain;

public sealed class UsuarioTests
{
    private static Usuario NovoUsuario(bool ativado = true) => new(
        "  Maria Silva  ",
        new Email("MARIA@EXEMPLO.COM"),
        new Telefone("11999999999"),
        new Cpf("52998224725"),
        "  MARIA  ",
        Senha.DeHash("hash"),
        RolePerson.Cliente,
        ativado,
        "  foto.png  ");

    [Fact]
    public void Construtor_Deve_Normalizar_Nome_Login_Email_E_Foto()
    {
        var u = NovoUsuario();
        Assert.Equal("Maria Silva", u.Nome);
        Assert.Equal("maria", u.Login);
        Assert.Equal("maria@exemplo.com", u.Email.Valor);
        Assert.Equal("foto.png", u.Foto);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Nome_Vazio_Deve_Falhar(string nome)
    {
        var ex = Assert.Throws<DomainException>(() => new Usuario(nome,new Email("a@b.com"),new Telefone("11999999999"),new Cpf("52998224725"),"login",Senha.DeHash("h"),RolePerson.Cliente,true,null));
        Assert.Equal("USER_INVALID_NAME", ex.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Login_Vazio_Deve_Falhar(string login)
    {
        var ex = Assert.Throws<DomainException>(() => new Usuario("Nome",new Email("a@b.com"),new Telefone("11999999999"),new Cpf("52998224725"),login,Senha.DeHash("h"),RolePerson.Cliente,true,null));
        Assert.Equal("USER_INVALID_LOGIN", ex.Code);
    }

    [Fact]
    public void Dependencias_Obrigatorias_Nulas_Devem_Falhar()
    {
        Assert.Equal("USER_INVALID_EMAIL", Assert.Throws<DomainException>(() => new Usuario("N",null!,new Telefone("11999999999"),new Cpf("52998224725"),"l",Senha.DeHash("h"),RolePerson.Cliente,true,null)).Code);
        Assert.Equal("USER_INVALID_PHONE", Assert.Throws<DomainException>(() => new Usuario("N",new Email("a@b.com"),null!,new Cpf("52998224725"),"l",Senha.DeHash("h"),RolePerson.Cliente,true,null)).Code);
        Assert.Equal("USER_INVALID_CPF", Assert.Throws<DomainException>(() => new Usuario("N",new Email("a@b.com"),new Telefone("11999999999"),null!,"l",Senha.DeHash("h"),RolePerson.Cliente,true,null)).Code);
        Assert.Equal("USER_INVALID_PASSWORD", Assert.Throws<DomainException>(() => new Usuario("N",new Email("a@b.com"),new Telefone("11999999999"),new Cpf("52998224725"),"l",null!,RolePerson.Cliente,true,null)).Code);
    }

    [Fact]
    public void Foto_Vazia_Deve_Virar_Null()
        => Assert.Null(new Usuario("N",new Email("a@b.com"),new Telefone("11999999999"),new Cpf("52998224725"),"l",Senha.DeHash("h"),RolePerson.Cliente,true,"   ").Foto);

    [Fact]
    public void RegistrarCriacao_Sem_Id_Deve_Falhar()
        => Assert.Equal("USER_INVALID_ID", Assert.Throws<DomainException>(NovoUsuario().RegistrarCriacao).Code);

    [Fact]
    public void RegistrarCriacao_Com_Id_Deve_Gerar_Evento()
    {
        var u = NovoUsuario();
        ReflectionHelper.SetPrivateProperty(u, "Id", 7);
        u.RegistrarCriacao();
        Assert.IsType<UsuarioCriadoDomainEvent>(Assert.Single(u.DomainEvents));
    }

    [Fact]
    public void AlterarDados_Deve_Atualizar_Valores()
    {
        var u = NovoUsuario();
        u.AlterarDados("  Novo Nome  ", new Email("novo@exemplo.com"), new Telefone("11888888888"), new Cpf("11144477735"));
        Assert.Equal("Novo Nome", u.Nome);
        Assert.Equal("novo@exemplo.com", u.Email.Valor);
        Assert.Equal("11888888888", u.Numero.Valor);
        Assert.Equal("11144477735", u.CPF.Valor);
    }

    [Fact]
    public void AlterarSenhaPerfil_Deve_Alterar_E_Gerar_Evento()
    {
        var u = NovoUsuario();
        u.AlterarSenhaPerfil(Senha.DeHash("novo-hash"));
        Assert.Equal("novo-hash", u.Senha.Hash);
        Assert.Contains(u.DomainEvents, e => e is SenhaAlteradaDomainEvent);
    }

    [Fact]
    public void Usuario_Ativo_Desbloqueado_Deve_Poder_Autenticar()
        => Assert.True(NovoUsuario().PodeAutenticar(new DateTime(2026,8,28,8,0,0)));

    [Fact]
    public void Usuario_Desativado_Nao_Deve_Poder_Autenticar()
        => Assert.False(NovoUsuario(false).PodeAutenticar(new DateTime(2026,8,28,8,0,0)));

    [Fact]
    public void Quatro_Falhas_Nao_Devem_Bloquear()
    {
        var u = NovoUsuario();
        var agora = new DateTime(2026,8,28,8,0,0);
        for (var i=0;i<4;i++) u.RegistrarFalhaLogin(agora);
        Assert.Equal(4,u.TentativasLogin);
        Assert.Null(u.BloqueioAte);
        Assert.True(u.PodeAutenticar(agora));
    }

    [Fact]
    public void Quinta_Falha_Deve_Bloquear_Por_Cinco_Minutos_E_Gerar_Evento()
    {
        var u = NovoUsuario();
        ReflectionHelper.SetPrivateProperty(u, "Id", 10);
        var agora = new DateTime(2026,8,28,8,0,0);
        for (var i=0;i<5;i++) u.RegistrarFalhaLogin(agora);
        Assert.Equal(5,u.TentativasLogin);
        Assert.Equal(agora.AddMinutes(5),u.BloqueioAte);
        Assert.False(u.PodeAutenticar(agora.AddMinutes(4)));
        Assert.True(u.PodeAutenticar(agora.AddMinutes(5)));
        Assert.Contains(u.DomainEvents, e => e is UsuarioBloqueadoDomainEvent);
    }

    [Fact]
    public void ResetarTentativas_Deve_Limpar_Contador_E_Bloqueio()
    {
        var u = NovoUsuario();
        var agora = new DateTime(2026,8,28,8,0,0);
        for (var i=0;i<5;i++) u.RegistrarFalhaLogin(agora);
        u.ResetarTentativasLogin();
        Assert.Equal(0,u.TentativasLogin);
        Assert.Null(u.BloqueioAte);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GerarCodigo_Vazio_Deve_Falhar(string codigo)
        => Assert.Equal("PASSWORD_RECOVERY_INVALID_CODE", Assert.Throws<DomainException>(() => NovoUsuario().GerarCodigo(codigo,DateTime.Now)).Code);

    [Fact]
    public void GerarCodigo_Deve_Ativar_Expirar_Em_15_Minutos_E_Gerar_Evento()
    {
        var u = NovoUsuario();
        var agora = new DateTime(2026,8,28,8,0,0);
        u.GerarCodigo(" 123456 ",agora);
        Assert.Equal("123456",u.Codigo);
        Assert.True(u.CodigoAtivo);
        Assert.Equal(0,u.TentativasCodigo);
        Assert.Equal(agora.AddMinutes(15),u.CodigoRecuperacaoExpiraEm);
        Assert.Contains(u.DomainEvents,e => e is RecuperacaoSenhaSolicitadaDomainEvent);
    }

    [Fact]
    public void Codigo_Deve_Valer_Antes_Do_Limite_E_Expirar_No_Limite()
    {
        var u = NovoUsuario();
        var agora = new DateTime(2026,8,28,8,0,0);
        u.GerarCodigo("123456",agora);
        Assert.True(u.CodigoIsValido(agora.AddMinutes(14).AddSeconds(59)));
        Assert.False(u.CodigoIsValido(agora.AddMinutes(15)));
    }

    [Fact]
    public void PodeTrocarSenha_Deve_Exigir_Codigo_Exato_E_Ativo()
    {
        var u = NovoUsuario();
        var agora = new DateTime(2026,8,28,8,0,0);
        u.GerarCodigo("123456",agora);
        Assert.True(u.PodeTrocarSenha(" 123456 ",agora.AddMinutes(1)));
        Assert.False(u.PodeTrocarSenha("654321",agora.AddMinutes(1)));
        Assert.False(u.PodeTrocarSenha("",agora.AddMinutes(1)));
    }

    [Fact]
    public void Quinta_Falha_De_Codigo_Deve_Desativar_Codigo()
    {
        var u = NovoUsuario();
        u.GerarCodigo("123456",DateTime.Now);
        for (var i=0;i<5;i++) u.RegistrarFalhaTrocaSenha();
        Assert.Equal(5,u.TentativasCodigo);
        Assert.False(u.CodigoAtivo);
    }

    [Fact]
    public void AlterarSenha_Deve_Limpar_Recuperacao_E_Gerar_Evento()
    {
        var u = NovoUsuario();
        u.GerarCodigo("123456",DateTime.Now);
        u.RegistrarFalhaTrocaSenha();
        u.AlterarSenha(Senha.DeHash("novo"));
        Assert.Equal("novo",u.Senha.Hash);
        Assert.Null(u.Codigo);
        Assert.Null(u.CodigoRecuperacaoExpiraEm);
        Assert.False(u.CodigoAtivo);
        Assert.Equal(0,u.TentativasCodigo);
        Assert.Contains(u.DomainEvents,e => e is SenhaAlteradaDomainEvent);
    }

    [Fact]
    public void Ativar_Usuario_Inativo_Deve_Ativar_E_Gerar_Evento()
    {
        var u = NovoUsuario(false);
        u.AtivarUsuario();
        Assert.True(u.Ativado);
        Assert.Contains(u.DomainEvents,e => e is UsuarioAtivacaoAlteradaDomainEvent);
    }

    [Fact]
    public void Ativar_Usuario_Ja_Ativo_Deve_Falhar()
        => Assert.Equal("USER_ALREADY_ACTIVE", Assert.Throws<DomainException>(NovoUsuario().AtivarUsuario).Code);

    [Fact]
    public void Desativar_Usuario_Ativo_Deve_Desativar_E_Gerar_Evento()
    {
        var u = NovoUsuario();
        u.DesativarUsuario();
        Assert.False(u.Ativado);
        Assert.Contains(u.DomainEvents,e => e is UsuarioAtivacaoAlteradaDomainEvent);
    }

    [Fact]
    public void Desativar_Usuario_Ja_Inativo_Deve_Falhar()
        => Assert.Equal("USER_ALREADY_INACTIVE", Assert.Throws<DomainException>(NovoUsuario(false).DesativarUsuario).Code);
}
