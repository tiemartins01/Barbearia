using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaInfrastructure.Data;
using BarbeariaInfrastructure.Repository;
using BarbeariaInfrastructure.Queries;
using BarbeariaTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BarbeariaTests.Infrastructure;

public sealed class InfrastructureTests : IDisposable
{
    private readonly AppDbContext _db;

    public InfrastructureTests()
    {
        var options=new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w=>w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _db=new AppDbContext(options);
    }

    public void Dispose()=>_db.Dispose();

    private static Agendamento NovoAgendamento(int id,int barbeiroId,DateTime inicio,int duracao=30)
    {
        var a=new Agendamento(1,barbeiroId,1,duracao,inicio,inicio.AddDays(-1));
        ReflectionHelper.SetPrivateProperty(a,"Id",id); return a;
    }

    [Fact]
    public void Modelo_Usuario_Deve_Ter_Indices_Unicos_Email_Telefone_Cpf_Login()
    {
        var unique=_db.Model.GetEntityTypes()
            .SelectMany(e=>e.GetIndexes())
            .Where(i=>i.IsUnique)
            .Select(i=>i.GetDatabaseName())
            .Where(x=>x is not null)
            .ToHashSet();
        Assert.Contains("ux_usuario_email",unique);
        Assert.Contains("ux_usuario_telefone",unique);
        Assert.Contains("ux_usuario_cpf",unique);
        Assert.Contains("ux_usuario_login",unique);
    }

    [Fact]
    public void Modelo_Avaliacao_Deve_Impedir_Duas_Avaliacoes_Para_Mesmo_Agendamento()
    {
        var entity=_db.Model.FindEntityType(typeof(Avaliacao))!;
        Assert.Contains(entity.GetIndexes(),i=>i.IsUnique && i.Properties.Select(p=>p.Name).SequenceEqual(new[]{"AgendamentoId"}));
    }

    [Fact]
    public void Modelo_Barbeiro_Deve_Ter_UsuarioId_Unico()
    {
        var entity=_db.Model.FindEntityType(typeof(Barbeiro))!;
        Assert.Contains(entity.GetIndexes(),i=>i.IsUnique && i.Properties.Select(p=>p.Name).SequenceEqual(new[]{"UsuarioId"}));
    }

    [Fact]
    public void Modelo_RefreshToken_Deve_Ter_Token_Unico()
    {
        var refreshType=_db.Model.GetEntityTypes().Single(x=>x.ClrType.Name=="RefreshToken");
        Assert.Contains(refreshType.GetIndexes(),i=>i.IsUnique && i.Properties.Any(p=>p.Name=="Token"));
    }

    [Fact]
    public void Modelo_Agendamento_Deve_Ter_Protecao_Definitiva_Contra_DoubleBooking_No_Banco()
    {
        var entity=_db.Model.FindEntityType(typeof(Agendamento))!;
        var existeIndiceUnico=entity.GetIndexes().Any(i=>i.IsUnique && i.Properties.Any(p=>p.Name=="BarbeiroId") && i.Properties.Any(p=>p.Name=="DataAgendamento"));
        // Contrato importante: hoje este teste revela a lacuna do índice comentado em AgendamentosConfiguration.
        Assert.True(existeIndiceUnico,"Falta índice/constraint única que proteja double booking no banco.");
    }

    [Fact]
    public async Task AgendamentoRepository_ExisteConflito_Deve_Detectar_Sobreposicao_De_Ativo()
    {
        var inicio=new DateTime(2026,9,1,10,0,0); _db.Agendamentos.Add(NovoAgendamento(1,5,inicio,60)); await _db.SaveChangesAsync();
        var repo=new AgendamentoRepository(_db);
        Assert.True(await repo.ExisteConflitoAsync(5,inicio.AddMinutes(30),inicio.AddMinutes(90)));
        Assert.False(await repo.ExisteConflitoAsync(5,inicio.AddMinutes(60),inicio.AddMinutes(90)));
        Assert.False(await repo.ExisteConflitoAsync(6,inicio.AddMinutes(30),inicio.AddMinutes(90)));
    }

    [Fact]
    public async Task AgendamentoRepository_Deve_Ignorar_Agendamento_Cancelado_No_Conflito()
    {
        var inicio=new DateTime(2026,9,1,10,0,0); var a=NovoAgendamento(1,5,inicio,60); a.Cancelar(); _db.Agendamentos.Add(a); await _db.SaveChangesAsync();
        Assert.False(await new AgendamentoRepository(_db).ExisteConflitoAsync(5,inicio.AddMinutes(30),inicio.AddMinutes(90)));
    }

    [Fact]
    public async Task AgendaDisponibilidadeQuery_Deve_Retornar_Somente_Ativos_Do_Barbeiro_No_Dia()
    {
        var data=new DateOnly(2026,9,1); var a1=NovoAgendamento(1,5,data.ToDateTime(new TimeOnly(10,0)),60); var a2=NovoAgendamento(2,5,data.ToDateTime(new TimeOnly(12,0)),30); a2.Cancelar(); var a3=NovoAgendamento(3,6,data.ToDateTime(new TimeOnly(14,0)),30); _db.Agendamentos.AddRange(a1,a2,a3); await _db.SaveChangesAsync();
        var result=await new AgendaDisponibilidadeQuery(_db).BuscarPeriodosOcupadosAsync(5,data);
        var periodo=Assert.Single(result); Assert.Equal(new TimeOnly(10,0),TimeOnly.FromDateTime(periodo.Inicio)); Assert.Equal(new TimeOnly(11,0),TimeOnly.FromDateTime(periodo.Fim));
    }

    [Fact]
    public void Infrastructure_Deve_Referenciar_Core()
    {
        var refs=typeof(AppDbContext).Assembly.GetReferencedAssemblies().Select(x=>x.Name).ToArray(); Assert.Contains(refs,x=>x?.Contains("BarbeariaCore")==true);
    }

    [Fact]
    public void PasswordHasher_TokenService_RefreshToken_Devem_Estar_Em_Infrastructure()
    {
        var asm=typeof(AppDbContext).Assembly; var names=asm.GetTypes().Select(t=>t.Name).ToHashSet(); Assert.Contains("PasswordHasher",names); Assert.Contains("TokenService",names); Assert.Contains("RefreshToken",names);
    }

    [Fact]
    public void Outbox_Idempotency_Audit_Devem_Existir_Na_Infrastructure()
    {
        var names=typeof(AppDbContext).Assembly.GetTypes().Select(t=>t.Name).ToHashSet(); Assert.Contains("OutboxMessage",names); Assert.Contains("IdempotencyRecord",names); Assert.Contains("AuditLog",names);
    }
}
