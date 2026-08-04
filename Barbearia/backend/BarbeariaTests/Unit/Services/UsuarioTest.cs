using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Domain.ValueObjects;
using Barbearia.Core.Enum;
using Barbearia.Core.Exceptions;

namespace Barbearia.Tests.Services
{
    public class UsuarioTest
    {
        // Usuário criado porque os itens são private e não pode ser criado além do próprio domain
        //private Usuario UsuarioTestes()
        //{
        //    return new Usuario(
        //        "admin",
        //        new Email("admin@gmail.com"),
        //        new Phone("27999999999"),
        //        new Cpf("22178204007"),
        //        "admin",
        //        Senha.Criar("123456"),
        //        RolePerson.Admin,
        //        true,
        //        null);
        //}

        //[Fact]
        //public void Deve_Lancar_Excecao_Quando_Nome_For_Vazio()
        //{
        //    var ex = Assert.Throws<DomainException>(() =>
        //        new Usuario(
        //            "", // vazio
        //            new Email("admin@gmail.com"),
        //            new Phone("27999999999"),
        //            new Cpf("22178204007"),
        //            "admin",
        //            Senha.Criar("123456"),
        //            RolePerson.Admin,
        //            true,
        //            null));

        //    Assert.Equal("Nome é obrigatório.", ex.Message);
        //}

        //[Fact]
        //public void Deve_Lancar_Excecao_Quando_Login_For_Vazio()
        //{
        //    var ex = Assert.Throws<DomainException>(() =>
        //        new Usuario(
        //            "admin",
        //            new Email("admin@gmail.com"),
        //            new Phone("27999999999"),
        //            new Cpf("22178204007"),
        //            "", // vazio
        //            Senha.Criar("123456"),
        //            RolePerson.Admin,
        //            true,
        //            null));

        //    Assert.Equal("Login é obrigatório.", ex.Message);
        //}

        //// QUANDO O USUÁRIO TENTA LOGAR COM AS CREDENCIAIS INVÁLIDAS 5 VEZES, ELE É BLOQUEADO 
        //[Fact]
        //public void Deve_Bloquear_Usuario_Apos_5_Falhas()
        //{
        //    var usuario = UsuarioTestes();

        //    while (usuario.TentativasLogin < 5)
        //    {
        //        usuario.RegistrarFalhaLogin();
        //    }

        //    Assert.False(usuario.PodeLogar());

        //    Assert.NotNull(usuario.BloqueioAte);
        //}

        //// RESETANDO AO CONSEGUIR ACESSAR O LOGIN
        //[Fact]
        //public void Deve_Resetar_Tentativas_Login()
        //{
        //    var usuario = UsuarioTestes();

        //    usuario.RegistrarFalhaLogin();

        //    usuario.RegistrarFalhaLogin();

        //    usuario.ResetarTentativasLogin();

        //    Assert.Equal(0, usuario.TentativasLogin);

        //    Assert.Null(usuario.BloqueioAte);
        //}
        //// GERANDO CÓDIGO PARA ALTERAR SENHA
        //[Fact]
        //public void Deve_Gerar_Codigo()
        //{
        //    var usuario = UsuarioTestes();

        //    usuario.GerarCodigo("123456");

        //    Assert.Equal("123456", usuario.Codigo);

        //    Assert.True(usuario.CodigoAtivo);

        //    Assert.NotNull(usuario.CodigoRecuperacaoExpiraEm);

        //    Assert.Equal(0, usuario.TentativasCodigo);
        //}
        //// CÓDIGO VÁLIDO PARA TROCAR DE SENHA
        //[Fact]
        //public void Deve_Retornar_Codigo_Valido()
        //{
        //    var usuario = UsuarioTestes();

        //    usuario.GerarCodigo("123456");

        //    Assert.True(usuario.CodigoIsValido());
        //}
        //// CASO A PESSOA ERRE O CÓDIGO 5 VEZES AO ALTERAR A SENHA, O CÓDIGO É BLOQUEADO
        //[Fact]
        //public void Deve_Invalidar_Codigo_Apos_5_Falhas()
        //{
        //    var usuario = UsuarioTestes();

        //    usuario.GerarCodigo("123456");

        //    while (usuario.TentativasCodigo < 5)
        //    {
        //        usuario.RegistrarFalhaTrocaSenha();
        //    }

        //    Assert.False(usuario.CodigoAtivo);
        //}
        //// CASO A PESSOA TENHA O CÓDIGO CORRETO, ELA PODE ALTERAR A SENHA
        //[Fact]
        //public void Deve_Permitir_Troca_Senha_Com_Codigo_Correto()
        //{
        //    var usuario = UsuarioTestes();

        //    usuario.GerarCodigo("123456");

        //    var resultado = usuario.PodeTrocarSenha("123456");

        //    Assert.True(resultado);
        //}
        //// EM CASO DE CÓDIGO INCORRETO, A SENHA NÃO PODE SER ALTERADA
        //[Fact]
        //public void Nao_Deve_Permitir_Troca_Senha_Com_Codigo_Incorreto()
        //{
        //    var usuario = UsuarioTestes();

        //    usuario.GerarCodigo("123456");

        //    var resultado = usuario.PodeTrocarSenha("123345");

        //    Assert.False(resultado);

        //}
        //// A SENHA PODE SER ALTERADA
        //[Fact]
        //public void Deve_Alterar_Senha()
        //{
        //    var usuario = UsuarioTestes();

        //    usuario.GerarCodigo("123456");

        //    usuario.AlterarSenha("654321");

        //    Assert.True(usuario.Senha.Verify("654321"));
        //}
        //// RESETA AS INFORMAÇÕES APÓS ALTERAR SENHA
        //[Fact]
        //public void Deve_Limpar_Codigo_Apos_Alterar_Senha()
        //{
        //    var usuario = UsuarioTestes();

        //    usuario.GerarCodigo("123456");

        //    usuario.AlterarSenha("654321");

        //    Assert.Null(usuario.Codigo);
        //    Assert.Null(usuario.CodigoRecuperacaoExpiraEm);
        //    Assert.Equal(0, usuario.TentativasCodigo);
        //    Assert.False(usuario.CodigoAtivo);
        //}
    }
}
