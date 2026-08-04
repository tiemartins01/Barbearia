//using Barbearia.Core.Domain.Entities;
//using Barbearia.Core.Domain.ValueObjects;
//using Barbearia.Core.Enum;
//using Barbearia.Core.Exceptions;

//namespace Barbearia.Tests.Domain;

//public class UsuarioRulesTests
//{
//    private static Usuario CriarUsuario() => new(
//        "Administrador",
//        new Email("admin@gmail.com"),
//        new Phone("27999999999"),
//        new Cpf("22178204007"),
//        "  ADMIN  ",
//        Senha.Criar("123456"),
//        RolePerson.Admin,
//        true,
//        null);

//    [Fact]
//    public void Deve_Normalizar_Login()
//    {
//        var usuario = CriarUsuario();
//        Assert.Equal("admin", usuario.Login);
//    }

//    [Fact]
//    public void Deve_Bloquear_Somente_Na_Quinta_Falha()
//    {
//        var usuario = CriarUsuario();

//        for (var i = 0; i < 4; i++)
//            usuario.RegistrarFalhaLogin();

//        Assert.True(usuario.PodeLogar());
//        Assert.Null(usuario.BloqueioAte);

//        usuario.RegistrarFalhaLogin();

//        Assert.False(usuario.PodeLogar());
//        Assert.NotNull(usuario.BloqueioAte);
//    }

//    [Fact]
//    public void GerarCodigo_Deve_Rejeitar_Valor_Vazio()
//    {
//        var usuario = CriarUsuario();

//        var ex = Assert.Throws<DomainException>(() => usuario.GerarCodigo(" "));

//        Assert.Equal("Código inválido.", ex.Message);
//    }

//    [Fact]
//    public void AlterarSenha_Deve_Rejeitar_Senha_Curta()
//    {
//        var usuario = CriarUsuario();

//        var ex = Assert.Throws<DomainException>(() => usuario.AlterarSenha("123"));

//        Assert.Contains("6 caracteres", ex.Message);
//    }
//}
