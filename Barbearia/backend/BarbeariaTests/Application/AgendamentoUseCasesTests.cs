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
    [InlineData(0, 1, 1, "BARBER_ID_INVALID")]
    [InlineData(1, 0, 1, "USER_ID_INVALID")]
    [InlineData(1, 1, 0, "SERVICE_ID_INVALID")]
    public async Task CriarAgendamento_Id_Invalido_Deve_Falhar(
        int barbeiro, int usuario, int servico, string code)
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var sut = new CriarAgendamento(
            Mock.Of<IAgendamentoRepository>(),
            Mock.Of<IBarbeiroRepository>(),
            Mock.Of<IServicoRepository>(),
            Mock.Of<IUnitOfWork>(),
            Mock.Of<ILogger<CriarAgendamento>>());

        var ex = await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ValidationException>(
            () => sut.ExecutarAsync(
                barbeiro, usuario, servico, DateTime.UtcNow.AddDays(1), token));

        Assert.Equal(code, ex.Code);
    }

    [Fact]
    public async Task CriarAgendamento_Barbeiro_Inexistente_Deve_Falhar()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var b = new Mock<IBarbeiroRepository>();
        b.Setup(x => x.ExisteAtivoAsync(1, token)).ReturnsAsync(false);

        var sut = new CriarAgendamento(
            Mock.Of<IAgendamentoRepository>(),
            b.Object,
            Mock.Of<IServicoRepository>(),
            Mock.Of<IUnitOfWork>(),
            Mock.Of<ILogger<CriarAgendamento>>());

        var ex = await Assert.ThrowsAsync<BarbeariaCore.Exceptions.NotFoundException>(
            () => sut.ExecutarAsync(1, 1, 1, DateTime.UtcNow.AddDays(1), token));

        Assert.Equal("BARBER_NOT_FOUND", ex.Code);
        b.Verify(x => x.ExisteAtivoAsync(1, token), Times.Once);
    }

    [Fact]
    public async Task CriarAgendamento_Servico_Inexistente_Deve_Falhar()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var b = new Mock<IBarbeiroRepository>();
        var s = new Mock<IServicoRepository>();

        b.Setup(x => x.ExisteAtivoAsync(1, token)).ReturnsAsync(true);
        s.Setup(x => x.ObterAtivoPorIdAsync(1, token)).ReturnsAsync((Servico?)null);

        var sut = new CriarAgendamento(
            Mock.Of<IAgendamentoRepository>(),
            b.Object,
            s.Object,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<ILogger<CriarAgendamento>>());

        var ex = await Assert.ThrowsAsync<BarbeariaCore.Exceptions.NotFoundException>(
            () => sut.ExecutarAsync(1, 1, 1, DateTime.UtcNow.AddDays(1), token));

        Assert.Equal("SERVICE_NOT_FOUND", ex.Code);
        b.Verify(x => x.ExisteAtivoAsync(1, token), Times.Once);
        s.Verify(x => x.ObterAtivoPorIdAsync(1, token), Times.Once);
    }

    [Fact]
    public async Task CriarAgendamento_Conflito_PreExistente_Deve_Virar_ConflictException()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var b = new Mock<IBarbeiroRepository>();
        var s = new Mock<IServicoRepository>();
        var a = new Mock<IAgendamentoRepository>();

        b.Setup(x => x.ExisteAtivoAsync(1, token)).ReturnsAsync(true);
        s.Setup(x => x.ObterAtivoPorIdAsync(1, token))
            .ReturnsAsync(new Servico("Corte", 30, 50, true));

        a.Setup(x => x.ExisteConflitoAsync(
                1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), token))
            .ReturnsAsync(true);

        var sut = new CriarAgendamento(
            a.Object, b.Object, s.Object,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<ILogger<CriarAgendamento>>());

        var ex = await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ConflictException>(
            () => sut.ExecutarAsync(1, 1, 1, DateTime.UtcNow.AddDays(2), token));

        Assert.Equal("APPOINTMENT_TIME_CONFLICT", ex.Code);

        a.Verify(x => x.ExisteConflitoAsync(
            1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), token), Times.Once);
    }

    [Fact]
    public async Task CriarAgendamento_Valido_Deve_Usar_Transacao_Salvar_Duas_Vezes_E_Commitar()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var b = new Mock<IBarbeiroRepository>();
        var s = new Mock<IServicoRepository>();
        var a = new Mock<IAgendamentoRepository>();
        var uow = new Mock<IUnitOfWork>();

        b.Setup(x => x.ExisteAtivoAsync(1, token)).ReturnsAsync(true);
        s.Setup(x => x.ObterAtivoPorIdAsync(1, token))
            .ReturnsAsync(new Servico("Corte", 30, 50, true));

        a.Setup(x => x.ExisteConflitoAsync(
                1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), token))
            .ReturnsAsync(false);

        a.Setup(x => x.AdicionarAsync(It.IsAny<Agendamento>(), token))
            .Callback<Agendamento, CancellationToken>((agendamento, _) =>
                ReflectionHelper.SetPrivateProperty(agendamento, "Id", 99))
            .Returns(Task.CompletedTask);

        var sut = new CriarAgendamento(
            a.Object, b.Object, s.Object, uow.Object,
            Mock.Of<ILogger<CriarAgendamento>>());

        var result = await sut.ExecutarAsync(
            1, 2, 1, DateTime.UtcNow.AddDays(2), token);

        Assert.True(result.Sucesso);

        b.Verify(x => x.ExisteAtivoAsync(1, token), Times.Once);
        s.Verify(x => x.ObterAtivoPorIdAsync(1, token), Times.Once);
        a.Verify(x => x.ExisteConflitoAsync(
            1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), token), Times.Once);
        a.Verify(x => x.AdicionarAsync(It.IsAny<Agendamento>(), token), Times.Once);

        uow.Verify(x => x.BeginTransactionAsync(token), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(token), Times.Exactly(2));
        uow.Verify(x => x.CommitTransactionAsync(token), Times.Once);
        uow.Verify(x => x.RollbackAsync(token), Times.Never);
    }

    [Fact]
    public async Task CriarAgendamento_Conflito_De_Persistencia_Deve_Rollback_E_Traduzir_Excecao()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var b = new Mock<IBarbeiroRepository>();
        var s = new Mock<IServicoRepository>();
        var a = new Mock<IAgendamentoRepository>();
        var uow = new Mock<IUnitOfWork>();

        b.Setup(x => x.ExisteAtivoAsync(1, token)).ReturnsAsync(true);
        s.Setup(x => x.ObterAtivoPorIdAsync(1, token))
            .ReturnsAsync(new Servico("Corte", 30, 50, true));

        a.Setup(x => x.ExisteConflitoAsync(
                1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), token))
            .ReturnsAsync(false);

        a.Setup(x => x.AdicionarAsync(It.IsAny<Agendamento>(), token))
            .ThrowsAsync(new PersistenceConflictException(
                "APPOINTMENT_TIME_CONFLICT", "x"));

        var sut = new CriarAgendamento(
            a.Object, b.Object, s.Object, uow.Object,
            Mock.Of<ILogger<CriarAgendamento>>());

        var ex = await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ConflictException>(
            () => sut.ExecutarAsync(1, 2, 1, DateTime.UtcNow.AddDays(2), token));

        Assert.Equal("APPOINTMENT_TIME_CONFLICT", ex.Code);
        uow.Verify(x => x.RollbackAsync(token), Times.Once);
        uow.Verify(x => x.CommitTransactionAsync(token), Times.Never);
    }

    [Fact]
    public async Task CriarAgendamento_Erro_Inesperado_Deve_Rollback_E_Propagar()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var b = new Mock<IBarbeiroRepository>();
        var s = new Mock<IServicoRepository>();
        var a = new Mock<IAgendamentoRepository>();
        var uow = new Mock<IUnitOfWork>();

        b.Setup(x => x.ExisteAtivoAsync(1, token)).ReturnsAsync(true);
        s.Setup(x => x.ObterAtivoPorIdAsync(1, token))
            .ReturnsAsync(new Servico("Corte", 30, 50, true));

        a.Setup(x => x.ExisteConflitoAsync(
                1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), token))
            .ReturnsAsync(false);

        a.Setup(x => x.AdicionarAsync(It.IsAny<Agendamento>(), token))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var sut = new CriarAgendamento(
            a.Object, b.Object, s.Object, uow.Object,
            Mock.Of<ILogger<CriarAgendamento>>());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ExecutarAsync(1, 2, 1, DateTime.UtcNow.AddDays(2), token));

        uow.Verify(x => x.RollbackAsync(token), Times.Once);
        uow.Verify(x => x.CommitTransactionAsync(token), Times.Never);
    }

    [Fact]
    public async Task ConsultarProximo_Id_Invalido_Deve_Falhar()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var sut = new ConsultarProximoAgendamento(
            Mock.Of<IProximoAgendamentoQuery>());

        var ex = await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ValidationException>(
            () => sut.ExecutarAsync(0, token));

        Assert.Equal("USER_ID_INVALID", ex.Code);
    }

    [Fact]
    public async Task ConsultarProximo_Deve_Delegar_Id_E_Agora_E_CancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var q = new Mock<IProximoAgendamentoQuery>();
        var sut = new ConsultarProximoAgendamento(q.Object);

        var antes = DateTime.UtcNow;

        await sut.ExecutarAsync(10, token);

        var depois = DateTime.UtcNow;

        q.Verify(x => x.ObterAsync(
            10,
            It.Is<DateTime>(data => data >= antes && data <= depois),
            token), Times.Once);
    }

    [Theory]
    [InlineData(0, 1, "BARBER_ID_INVALID")]
    [InlineData(1, 0, "SERVICE_ID_INVALID")]
    public async Task ConsultarHorarios_Id_Invalido_Deve_Falhar(
        int barbeiro, int servico, string code)
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var sut = new ConsultarHorariosDisponiveis(
            Mock.Of<IBarbeiroRepository>(),
            Mock.Of<IServicoRepository>(),
            Mock.Of<IAgendaDisponibilidadeQuery>());

        var ex = await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ValidationException>(
            () => sut.ExecutarAsync(
                barbeiro,
                servico,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                token));

        Assert.Equal(code, ex.Code);
    }

    [Fact]
    public async Task ConsultarHorarios_Barbeiro_Inexistente_Deve_Falhar()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var b = new Mock<IBarbeiroRepository>();
        b.Setup(x => x.ExisteAtivoAsync(1, token)).ReturnsAsync(false);

        var sut = new ConsultarHorariosDisponiveis(
            b.Object,
            Mock.Of<IServicoRepository>(),
            Mock.Of<IAgendaDisponibilidadeQuery>());

        var ex = await Assert.ThrowsAsync<BarbeariaCore.Exceptions.NotFoundException>(
            () => sut.ExecutarAsync(
                1,
                1,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                token));

        Assert.Equal("BARBER_NOT_FOUND", ex.Code);
        b.Verify(x => x.ExisteAtivoAsync(1, token), Times.Once);
    }

    [Fact]
    public async Task ConsultarHorarios_Deve_Remover_Slots_Com_Sobreposicao_E_Que_Nao_Cabem_No_Expediente()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var b = new Mock<IBarbeiroRepository>();
        var s = new Mock<IServicoRepository>();
        var q = new Mock<IAgendaDisponibilidadeQuery>();

        var data = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        b.Setup(x => x.ExisteAtivoAsync(1, token)).ReturnsAsync(true);

        s.Setup(x => x.ObterAtivoPorIdAsync(1, token))
            .ReturnsAsync(new Servico("Corte", 60, 50, true));

        q.Setup(x => x.BuscarPeriodosOcupadosAsync(1, data, token))
            .ReturnsAsync(new[]
            {
                new PeriodoOcupado(
                    data.ToDateTime(new TimeOnly(10, 0)),
                    data.ToDateTime(new TimeOnly(11, 0)))
            });

        var sut = new ConsultarHorariosDisponiveis(
            b.Object, s.Object, q.Object);

        var result = await sut.ExecutarAsync(1, 1, data, token);

        Assert.DoesNotContain(new TimeOnly(9, 30), result);
        Assert.DoesNotContain(new TimeOnly(10, 0), result);
        Assert.DoesNotContain(new TimeOnly(10, 30), result);
        Assert.DoesNotContain(new TimeOnly(17, 30), result);
        Assert.Contains(new TimeOnly(11, 0), result);

        b.Verify(x => x.ExisteAtivoAsync(1, token), Times.Once);
        s.Verify(x => x.ObterAtivoPorIdAsync(1, token), Times.Once);
        q.Verify(x => x.BuscarPeriodosOcupadosAsync(1, data, token), Times.Once);
    }

    [Fact]
    public async Task CriarAgendamento_Deve_Propagar_CancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var b = new Mock<IBarbeiroRepository>();
        var s = new Mock<IServicoRepository>();
        var a = new Mock<IAgendamentoRepository>();

        b.Setup(x => x.ExisteAtivoAsync(1, token)).ReturnsAsync(true);

        s.Setup(x => x.ObterAtivoPorIdAsync(1, token))
            .ReturnsAsync(new Servico("Corte", 30, 50, true));

        a.Setup(x => x.ExisteConflitoAsync(
                1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), token))
            .ReturnsAsync(true);

        var sut = new CriarAgendamento(
            a.Object,
            b.Object,
            s.Object,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<ILogger<CriarAgendamento>>());

        await Assert.ThrowsAsync<BarbeariaCore.Exceptions.ConflictException>(
            () => sut.ExecutarAsync(
                1, 2, 1, DateTime.UtcNow.AddDays(2), token));

        b.Verify(x => x.ExisteAtivoAsync(1, token), Times.Once);
        s.Verify(x => x.ObterAtivoPorIdAsync(1, token), Times.Once);
        a.Verify(x => x.ExisteConflitoAsync(
            1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), token), Times.Once);
    }
}
