using BarbeariaCore.Application.Exceptions;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Application.Models;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.UseCases.Agendamentos;
using BarbeariaTests.Helpers;
using Microsoft.Extensions.Logging;

namespace BarbeariaTests.Application;

public sealed class AgendamentoUseCasesTests
{
    [Theory]
    [InlineData(0,1,1,"BARBER_ID_INVALID")]
    [InlineData(1,0,1,"USER_ID_INVALID")]
    [InlineData(1,1,0,"SERVICE_ID_INVALID")]
    public async Task CriarAgendamento_Id_Invalido_Deve_Falhar(int barbeiro,int usuario,int servico,string code)
    {
        var sut=new CriarAgendamento(Mock.Of<IAgendamentoRepository>(),Mock.Of<IBarbeiroRepository>(),Mock.Of<IServicoRepository>(),Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<CriarAgendamento>>());
        var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ValidationException>(()=>sut.ExecutarAsync(barbeiro,usuario,servico,DateTime.Now.AddDays(1))); Assert.Equal(code,ex.Code);
    }

    [Fact]
    public async Task CriarAgendamento_Barbeiro_Inexistente_Deve_Falhar()
    {
        var b=new Mock<IBarbeiroRepository>(); b.Setup(x=>x.ExisteAtivoAsync(1)).ReturnsAsync(false);
        var sut=new CriarAgendamento(Mock.Of<IAgendamentoRepository>(),b.Object,Mock.Of<IServicoRepository>(),Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<CriarAgendamento>>());
        Assert.Equal("BARBER_NOT_FOUND",(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.NotFoundException>(()=>sut.ExecutarAsync(1,1,1,DateTime.Now.AddDays(1)))).Code);
    }

    [Fact]
    public async Task CriarAgendamento_Servico_Inexistente_Deve_Falhar()
    {
        var b=new Mock<IBarbeiroRepository>(); b.Setup(x=>x.ExisteAtivoAsync(1)).ReturnsAsync(true); var s=new Mock<IServicoRepository>();
        var sut=new CriarAgendamento(Mock.Of<IAgendamentoRepository>(),b.Object,s.Object,Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<CriarAgendamento>>());
        Assert.Equal("SERVICE_NOT_FOUND",(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.NotFoundException>(()=>sut.ExecutarAsync(1,1,1,DateTime.Now.AddDays(1)))).Code);
    }

    [Fact]
    public async Task CriarAgendamento_Conflito_PreExistente_Deve_Virar_ConflictException()
    {
        var b=new Mock<IBarbeiroRepository>(); b.Setup(x=>x.ExisteAtivoAsync(1)).ReturnsAsync(true); var s=new Mock<IServicoRepository>(); s.Setup(x=>x.ObterAtivoPorIdAsync(1)).ReturnsAsync(new Servico("Corte",30,50,true)); var a=new Mock<IAgendamentoRepository>(); a.Setup(x=>x.ExisteConflitoAsync(1,It.IsAny<DateTime>(),It.IsAny<DateTime>())).ReturnsAsync(true);
        var sut=new CriarAgendamento(a.Object,b.Object,s.Object,Mock.Of<IUnitOfWork>(),Mock.Of<ILogger<CriarAgendamento>>());
        Assert.Equal("APPOINTMENT_TIME_CONFLICT",(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ConflictException>(()=>sut.ExecutarAsync(1,1,1,DateTime.Today.AddDays(2).AddHours(10)))).Code);
    }

    [Fact]
    public async Task CriarAgendamento_Valido_Deve_Usar_Transacao_Salvar_Duas_Vezes_E_Commitar()
    {
        var b=new Mock<IBarbeiroRepository>(); b.Setup(x=>x.ExisteAtivoAsync(1)).ReturnsAsync(true); var s=new Mock<IServicoRepository>(); s.Setup(x=>x.ObterAtivoPorIdAsync(1)).ReturnsAsync(new Servico("Corte",30,50,true)); var a=new Mock<IAgendamentoRepository>(); a.Setup(x=>x.AdicionarAsync(It.IsAny<Agendamento>())).Callback<Agendamento>(x=>ReflectionHelper.SetPrivateProperty(x,"Id",99)).Returns(Task.CompletedTask); var uow=new Mock<IUnitOfWork>();
        var sut=new CriarAgendamento(a.Object,b.Object,s.Object,uow.Object,Mock.Of<ILogger<CriarAgendamento>>()); var result=await sut.ExecutarAsync(1,2,1,DateTime.Today.AddDays(2).AddHours(10));
        Assert.True(result.Sucesso); uow.Verify(x=>x.BeginTransactionAsync(),Times.Once); uow.Verify(x=>x.SaveChangesAsync(),Times.Exactly(2)); uow.Verify(x=>x.CommitTransactionAsync(),Times.Once); uow.Verify(x=>x.RollbackAsync(),Times.Never);
    }

    [Fact]
    public async Task CriarAgendamento_Conflito_De_Persistencia_Deve_Rollback_E_Traduzir_Excecao()
    {
        var b=new Mock<IBarbeiroRepository>(); b.Setup(x=>x.ExisteAtivoAsync(1)).ReturnsAsync(true); var s=new Mock<IServicoRepository>(); s.Setup(x=>x.ObterAtivoPorIdAsync(1)).ReturnsAsync(new Servico("Corte",30,50,true)); var a=new Mock<IAgendamentoRepository>(); a.Setup(x=>x.AdicionarAsync(It.IsAny<Agendamento>())).ThrowsAsync(new PersistenceConflictException("APPOINTMENT_TIME_CONFLICT","x")); var uow=new Mock<IUnitOfWork>();
        var sut=new CriarAgendamento(a.Object,b.Object,s.Object,uow.Object,Mock.Of<ILogger<CriarAgendamento>>()); var ex=await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ConflictException>(()=>sut.ExecutarAsync(1,2,1,DateTime.Today.AddDays(2).AddHours(10))); Assert.Equal("APPOINTMENT_TIME_CONFLICT",ex.Code); uow.Verify(x=>x.RollbackAsync(),Times.Once);
    }

    [Fact]
    public async Task CriarAgendamento_Erro_Inesperado_Deve_Rollback_E_Propagar()
    {
        var b=new Mock<IBarbeiroRepository>(); b.Setup(x=>x.ExisteAtivoAsync(1)).ReturnsAsync(true); var s=new Mock<IServicoRepository>(); s.Setup(x=>x.ObterAtivoPorIdAsync(1)).ReturnsAsync(new Servico("Corte",30,50,true)); var a=new Mock<IAgendamentoRepository>(); a.Setup(x=>x.AdicionarAsync(It.IsAny<Agendamento>())).ThrowsAsync(new InvalidOperationException("boom")); var uow=new Mock<IUnitOfWork>();
        var sut=new CriarAgendamento(a.Object,b.Object,s.Object,uow.Object,Mock.Of<ILogger<CriarAgendamento>>()); await Assert.ThrowsAsync<InvalidOperationException>(()=>sut.ExecutarAsync(1,2,1,DateTime.Today.AddDays(2).AddHours(10))); uow.Verify(x=>x.RollbackAsync(),Times.Once);
    }

    [Fact]
    public async Task ConsultarProximo_Id_Invalido_Deve_Falhar()
    {
        var sut=new ConsultarProximoAgendamento(Mock.Of<IProximoAgendamentoQuery>()); Assert.Equal("USER_ID_INVALID",(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ValidationException>(()=>sut.ExecutarAsync(0))).Code);
    }

    [Fact]
    public async Task ConsultarProximo_Deve_Delegar_Id_E_Agora()
    {
        var q=new Mock<IProximoAgendamentoQuery>(); await new ConsultarProximoAgendamento(q.Object).ExecutarAsync(10); q.Verify(x=>x.ObterAsync(10,It.Is<DateTime>(d=>d>DateTime.Now.AddMinutes(-1))),Times.Once);
    }

    [Theory]
    [InlineData(0,1,"BARBER_ID_INVALID")]
    [InlineData(1,0,"SERVICE_ID_INVALID")]
    public async Task ConsultarHorarios_Id_Invalido_Deve_Falhar(int barbeiro,int servico,string code)
    {
        var sut=new ConsultarHorariosDisponiveis(Mock.Of<IBarbeiroRepository>(),Mock.Of<IServicoRepository>(),Mock.Of<IAgendaDisponibilidadeQuery>()); Assert.Equal(code,(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ValidationException>(()=>sut.ExecutarAsync(barbeiro,servico,DateOnly.FromDateTime(DateTime.Today.AddDays(1))))).Code);
    }

    [Fact]
    public async Task ConsultarHorarios_Barbeiro_Inexistente_Deve_Falhar()
    {
        var b=new Mock<IBarbeiroRepository>(); b.Setup(x=>x.ExisteAtivoAsync(1)).ReturnsAsync(false); var sut=new ConsultarHorariosDisponiveis(b.Object,Mock.Of<IServicoRepository>(),Mock.Of<IAgendaDisponibilidadeQuery>()); Assert.Equal("BARBER_NOT_FOUND",(await Assert.ThrowsAsync<BarbeariaCore.Exceptions.NotFoundException>(()=>sut.ExecutarAsync(1,1,DateOnly.FromDateTime(DateTime.Today.AddDays(1))))).Code);
    }

    [Fact]
    public async Task ConsultarHorarios_Deve_Remover_Slots_Com_Sobreposicao_E_Que_Nao_Cabem_No_Expediente()
    {
        var b=new Mock<IBarbeiroRepository>(); b.Setup(x=>x.ExisteAtivoAsync(1)).ReturnsAsync(true); var s=new Mock<IServicoRepository>(); s.Setup(x=>x.ObterAtivoPorIdAsync(1)).ReturnsAsync(new Servico("Corte",60,50,true)); var q=new Mock<IAgendaDisponibilidadeQuery>(); var data=DateOnly.FromDateTime(DateTime.Today.AddDays(2)); q.Setup(x=>x.BuscarPeriodosOcupadosAsync(1,data)).ReturnsAsync(new[]{new PeriodoOcupado(data.ToDateTime(new TimeOnly(10,0)),data.ToDateTime(new TimeOnly(11,0)))});
        var result=await new ConsultarHorariosDisponiveis(b.Object,s.Object,q.Object).ExecutarAsync(1,1,data);
        Assert.DoesNotContain(new TimeOnly(9,30),result); Assert.DoesNotContain(new TimeOnly(10,0),result); Assert.DoesNotContain(new TimeOnly(10,30),result); Assert.DoesNotContain(new TimeOnly(17,30),result); Assert.Contains(new TimeOnly(11,0),result);
    }
}
