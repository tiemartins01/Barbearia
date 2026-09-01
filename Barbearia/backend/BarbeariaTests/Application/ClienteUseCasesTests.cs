using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaCore.UseCases.Cliente;
using BarbeariaTests.Helpers;
using Microsoft.Extensions.Logging;

namespace BarbeariaTests.Application;

public sealed class ClienteUseCasesTests
{
    private static Usuario UsuarioValido()
    {
        var u=new Usuario("Maria",new Email("maria@exemplo.com"),new Telefone("11999999999"),new Cpf("52998224725"),"maria",Senha.DeHash("hash"),RolePerson.Cliente,true,null);
        ReflectionHelper.SetPrivateProperty(u,"Id",1); return u;
    }

    private static Agendamento AgendamentoValido(int clienteId=1)
    {
        var a=new Agendamento(clienteId,2,3,30,new DateTime(2026,9,1,10,0,0),new DateTime(2026,8,28,8,0,0));
        ReflectionHelper.SetPrivateProperty(a,"Id",20); return a;
    }

    [Fact]
    public async Task AlterarDados_Usuario_Inexistente_Deve_Falhar()
    {
        var repo=new Mock<IUsuarioRepository>();
        var sut=new AlterarDadosPessoais(Mock.Of<IUnitOfWork>(),repo.Object,Mock.Of<IPasswordHash>());
        await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync(new DTOAlterandoDados{Id=1,Nome="N",Email="a@b.com",Telefone="11999999999",Cpf="52998224725"}));
    }

    [Fact]
    public async Task AlterarDados_Sem_NovaSenha_Deve_Atualizar_Dados_E_Salvar_Sem_Hash()
    {
        var u=UsuarioValido(); var repo=new Mock<IUsuarioRepository>(); repo.Setup(x=>x.ObterPorIdAsync(1)).ReturnsAsync(u); var hash=new Mock<IPasswordHash>(); var uow=new Mock<IUnitOfWork>();
        var sut=new AlterarDadosPessoais(uow.Object,repo.Object,hash.Object);
        await sut.ExecutarAsync(new DTOAlterandoDados{Id=1,Nome="Novo",Email="novo@exemplo.com",Telefone="11888888888",Cpf="11144477735",NovaSenha=""});
        Assert.Equal("Novo",u.Nome); hash.Verify(x=>x.Hash(It.IsAny<string>()),Times.Never); repo.Verify(x=>x.AtualizarAsync(u),Times.Once); uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Fact]
    public async Task AlterarDados_Com_SenhaAntiga_Incorreta_Deve_Falhar_Sem_Salvar()
    {
        var u=UsuarioValido(); var repo=new Mock<IUsuarioRepository>(); repo.Setup(x=>x.ObterPorIdAsync(1)).ReturnsAsync(u); var hash=new Mock<IPasswordHash>(); hash.Setup(x=>x.Verify("errada","hash")).Returns(false); var uow=new Mock<IUnitOfWork>();
        var sut=new AlterarDadosPessoais(uow.Object,repo.Object,hash.Object);
        await Assert.ThrowsAsync<BarbeariaCore.Exceptions.AuthenticationException>(()=>sut.ExecutarAsync(new DTOAlterandoDados{Id=1,Nome="N",Email="a@b.com",Telefone="11999999999",Cpf="52998224725",SenhaAntiga="errada",NovaSenha="abcdef"}));
        uow.Verify(x=>x.SaveChangesAsync(),Times.Never);
    }

    [Fact]
    public async Task AlterarDados_Com_NovaSenha_Valida_Deve_Hashear_E_Gerar_Evento()
    {
        var u=UsuarioValido(); var repo=new Mock<IUsuarioRepository>(); repo.Setup(x=>x.ObterPorIdAsync(1)).ReturnsAsync(u); var hash=new Mock<IPasswordHash>(); hash.Setup(x=>x.Verify("antiga","hash")).Returns(true); hash.Setup(x=>x.Hash("abcdef")).Returns("novo");
        var sut=new AlterarDadosPessoais(Mock.Of<IUnitOfWork>(),repo.Object,hash.Object);
        await sut.ExecutarAsync(new DTOAlterandoDados{Id=1,Nome="N",Email="a@b.com",Telefone="11999999999",Cpf="52998224725",SenhaAntiga="antiga",NovaSenha="abcdef"});
        Assert.Equal("novo",u.Senha.Hash); Assert.NotEmpty(u.DomainEvents);
    }

    [Fact]
    public async Task Avaliar_Agendamento_Inexistente_Deve_Falhar()
    {
        var repo=new Mock<IAgendamentoRepository>(); var sut=new AvaliarAtendimento(repo.Object,Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<AvaliarAtendimento>>(),Mock.Of<IAvaliacaoRepository>());
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.NotFoundException>(()=>sut.ExecutarAsync(new DTOAvaliacao{AgendamentoId=20,Nota=5},1)); Assert.Equal("APPOINTMENT_NOT_FOUND",ex.Code);
    }

    [Fact]
    public async Task Avaliar_Agendamento_De_Outro_Cliente_Deve_Falhar()
    {
        var a=AgendamentoValido(2); var repo=new Mock<IAgendamentoRepository>(); repo.Setup(x=>x.ObterPorIdAsync(20)).ReturnsAsync(a);
        var sut=new AvaliarAtendimento(repo.Object,Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<AvaliarAtendimento>>(),Mock.Of<IAvaliacaoRepository>());
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ForbiddenException>(()=>sut.ExecutarAsync(new DTOAvaliacao{AgendamentoId=20,Nota=5},1)); Assert.Equal("RESOURCE_ACCESS_DENIED",ex.Code);
    }

    [Fact]
    public async Task Avaliar_Duplicado_Deve_Falhar()
    {
        var a=AgendamentoValido(); a.Concluir(); var repo=new Mock<IAgendamentoRepository>(); repo.Setup(x=>x.ObterPorIdAsync(20)).ReturnsAsync(a); var aval=new Mock<IAvaliacaoRepository>(); aval.Setup(x=>x.ExisteParaAgendamentoAsync(20)).ReturnsAsync(true);
        var sut=new AvaliarAtendimento(repo.Object,Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<AvaliarAtendimento>>(),aval.Object);
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ConflictException>(()=>sut.ExecutarAsync(new DTOAvaliacao{AgendamentoId=20,Nota=5},1)); Assert.Equal("REVIEW_ALREADY_EXISTS",ex.Code);
    }

    [Fact]
    public async Task Avaliar_Agendamento_Nao_Concluido_Deve_Propagar_Regra_Do_Dominio()
    {
        var a=AgendamentoValido(); var repo=new Mock<IAgendamentoRepository>(); repo.Setup(x=>x.ObterPorIdAsync(20)).ReturnsAsync(a); var aval=new Mock<IAvaliacaoRepository>();
        var sut=new AvaliarAtendimento(repo.Object,Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<AvaliarAtendimento>>(),aval.Object);
        var ex=await Assert.ThrowsAsync<DomainException>(()=>sut.ExecutarAsync(new DTOAvaliacao{AgendamentoId=20,Nota=5},1)); Assert.Equal("REVIEW_INVALID_APPOINTMENT_STATUS",ex.Code);
    }

    [Fact]
    public async Task Avaliar_Concluido_Deve_Adicionar_Avaliacao_Marcar_Avaliado_E_Salvar()
    {
        var a=AgendamentoValido(); a.Concluir(); var repo=new Mock<IAgendamentoRepository>(); repo.Setup(x=>x.ObterPorIdAsync(20)).ReturnsAsync(a); var aval=new Mock<IAvaliacaoRepository>(); var uow=new Mock<IUnitOfWork>();
        var sut=new AvaliarAtendimento(repo.Object,uow.Object,Mock.Of<ILogger<AvaliarAtendimento>>(),aval.Object);
        await sut.ExecutarAsync(new DTOAvaliacao{AgendamentoId=20,Nota=5,Comentario=" ótimo "},1);
        Assert.Equal(StatusAgendamento.Avaliado,a.Status); aval.Verify(x=>x.AdicionarAsync(It.Is<Avaliacao>(r=>r.Nota==5 && r.Comentario=="ótimo")),Times.Once); uow.Verify(x=>x.SaveChangesAsync(),Times.Once);
    }

    [Fact]
    public async Task ConsultarAgendamento_Inexistente_Deve_Retornar_Null()
    {
        var repo=new Mock<IAgendamentoRepository>(); var sut=new ConsultarAgendamento(repo.Object); Assert.Null(await sut.ExecutarAsync(1,999));
    }

    [Fact]
    public async Task ConsultarAgendamento_Existente_Deve_Mapear_Dados()
    {
        var a=AgendamentoValido(); var repo=new Mock<IAgendamentoRepository>(); repo.Setup(x=>x.ObterPorIdAsync(20)).ReturnsAsync(a); var dto=await new ConsultarAgendamento(repo.Object).ExecutarAsync(20,999);
        Assert.NotNull(dto); Assert.Equal(20,dto!.Id); Assert.Equal(1,dto.IdCliente); Assert.Equal(2,dto.IdBarbeiro); Assert.Equal(3,dto.IdServico);
    }

    [Fact]
    public async Task ConsultarAgendamentoDoCliente_De_Outro_Cliente_Deve_Falhar()
    {
        var a=AgendamentoValido(2); var repo=new Mock<IAgendamentoRepository>(); repo.Setup(x=>x.ObterPorIdAsync(20)).ReturnsAsync(a);
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ForbiddenException>(()=>new ConsultarAgendamentoDoCliente(repo.Object).ExecutarAsync(20,1)); Assert.Equal("RESOURCE_ACCESS_DENIED",ex.Code);
    }

    [Fact]
    public async Task ConsultarDadosPessoais_Deve_Delegar_Para_Query()
    {
        var q=new Mock<IDadosPessoaisQuery>(); var expected=new DTODadosPessoais{Id=1}; q.Setup(x=>x.ConsultarAsync(1)).ReturnsAsync(expected);
        Assert.Same(expected,await new ConsultarDadosPessoais(q.Object).ExecutarAsync(1));
    }

    [Fact]
    public async Task ConsultarHistorico_Deve_Propagar_Paginacao()
    {
        var q=new Mock<IHistoricoClienteQuery>(); q.Setup(x=>x.ConsultarAsync(1,2,20)).ReturnsAsync(Array.Empty<DTOHistorico>());
        await new ConsultarHistoricoCliente(q.Object).ExecutarAsync(1,2,20); q.Verify(x=>x.ConsultarAsync(1,2,20),Times.Once);
    }

    [Fact]
    public async Task ListarBarbeiros_Deve_Delegar_Para_Query()
    {
        var q=new Mock<IBarbeirosQuery>(); q.Setup(x=>x.ListarAtivosAsync()).ReturnsAsync(new[]{new DTOBarbeiro{Id=1}});
        var result=await new ListarBarbeiros(q.Object).ExecutarAsync(); Assert.Single(result);
    }
}
