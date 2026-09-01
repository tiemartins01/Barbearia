using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Validation;

namespace BarbeariaTests.Validation;

public sealed class ValidatorsTests
{
    [Fact]
    public void Login_Valido_Deve_Passar()
        => Assert.True(new DTOLoginUsuarioValidator().Validate(new DTOLoginUsuario{Nome="maria",Senha="123456"}).IsValid);

    [Theory]
    [InlineData("", "123456")]
    [InlineData("maria", "")]
    public void Login_Campos_Obrigatorios_Devem_Falhar(string login,string senha)
        => Assert.False(new DTOLoginUsuarioValidator().Validate(new DTOLoginUsuario{Nome=login,Senha=senha}).IsValid);

    [Fact]
    public void Login_Acima_Dos_Limites_Deve_Falhar()
        => Assert.False(new DTOLoginUsuarioValidator().Validate(new DTOLoginUsuario{Nome=new string('a',51),Senha=new string('b',129)}).IsValid);

    [Fact]
    public void NovoUsuario_Valido_Deve_Passar()
        => Assert.True(new DTONovoUsuarioValidator().Validate(new DTONovoUsuario{Nome="Maria",Email="m@e.com",Phone="11999999999",CPF="52998224725",Login="maria",SenhaR="123456"}).IsValid);

    [Fact]
    public void NovoUsuario_Campos_Obrigatorios_Devem_Falhar()
        => Assert.False(new DTONovoUsuarioValidator().Validate(new DTONovoUsuario()).IsValid);

    [Fact]
    public void NovoUsuario_Senha_Com_Menos_De_6_Deve_Falhar()
        => Assert.False(new DTONovoUsuarioValidator().Validate(new DTONovoUsuario{Nome="Maria",Email="m@e.com",Phone="11999999999",CPF="52998224725",Login="maria",SenhaR="12345"}).IsValid);

    [Fact]
    public void NovoUsuario_Foto_Acima_De_500_Deve_Falhar()
        => Assert.False(new DTONovoUsuarioValidator().Validate(new DTONovoUsuario{Nome="Maria",Email="m@e.com",Phone="11999999999",CPF="52998224725",Login="maria",SenhaR="123456",Foto=new string('a',501)}).IsValid);

    [Fact]
    public void AlterarDados_Valido_Deve_Passar()
        => Assert.True(new DTOAlterandoDadosValidator().Validate(new DTOAlterandoDados{Id=1,Nome="Maria",Email="m@e.com",Telefone="11999999999",Cpf="52998224725",SenhaAntiga="antiga",NovaSenha="123456"}).IsValid);

    [Fact]
    public void AlterarDados_Id_Invalido_Deve_Falhar()
        => Assert.False(new DTOAlterandoDadosValidator().Validate(new DTOAlterandoDados{Id=0,Nome="Maria",Email="m@e.com",Telefone="11999999999",Cpf="52998224725",SenhaAntiga="antiga",NovaSenha="123456"}).IsValid);

    [Theory]
    [InlineData(0,true)]
    [InlineData(1,true)]
    [InlineData(5,true)]
    [InlineData(6,false)]
    public void Avaliacao_Nota_Deve_Ficar_Entre_1_E_5(int nota,bool invalido)
    {
        var result=new DTOAvaliacaoValidator().Validate(new DTOAvaliacao{AgendamentoId=1,Nota=nota});
        Assert.Equal(invalido,!result.IsValid);
    }

    [Fact]
    public void Avaliacao_Comentario_Acima_De_128_Deve_Falhar()
        => Assert.False(new DTOAvaliacaoValidator().Validate(new DTOAvaliacao{Nota=5,Comentario=new string('a',129)}).IsValid);

    [Fact]
    public void EnviarEmail_Vazio_Deve_Falhar()
        => Assert.False(new DTOEnviarEmailValidator().Validate(new DTOEnviarEmail{Email=""}).IsValid);

    [Fact]
    public void MarcarHorario_Ids_Invalidos_Deve_Falhar()
        => Assert.False(new DTOMarcarHorarioValidator().Validate(new DTOMarcarHorario{Id_barbeiro=0,Id_servico=0,horario=default}).IsValid);

    [Fact]
    public void MudarSenha_Valido_Deve_Passar()
        => Assert.True(new DTOMudarSenhaValidator().Validate(new DTOMudarSenha{Email="m@e.com",Codigo="123456",Senha="abcdef",SenhaRepetida="abcdef"}).IsValid);

    [Fact]
    public void MudarSenha_Codigo_Que_Nao_Tem_6_Deve_Falhar()
        => Assert.False(new DTOMudarSenhaValidator().Validate(new DTOMudarSenha{Email="m@e.com",Codigo="12345",Senha="abcdef",SenhaRepetida="abcdef"}).IsValid);

    [Fact]
    public void MudarSenha_Senhas_Diferentes_Devem_Falhar()
        => Assert.False(new DTOMudarSenhaValidator().Validate(new DTOMudarSenha{Email="m@e.com",Codigo="123456",Senha="abcdef",SenhaRepetida="abcdefg"}).IsValid);
}
