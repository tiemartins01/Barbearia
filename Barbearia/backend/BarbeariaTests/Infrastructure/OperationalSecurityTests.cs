using BarbeariaCore.Infrastructure.Data.Operational;
using BarbeariaInfrastructure.Security;

namespace BarbeariaTests.Infrastructure;

public sealed class OperationalSecurityTests
{
    [Fact]
    public void PasswordHasher_Deve_Gerar_Hash_E_Verificar_Senha_Correta()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("Senha@123");
        Assert.NotEqual("Senha@123", hash);
        Assert.True(hasher.Verify("Senha@123", hash));
        Assert.False(hasher.Verify("outra", hash));
    }

    [Fact]
    public void RefreshToken_Deve_Identificar_Expiracao_E_Atividade()
    {
        var agora = DateTime.UtcNow;
        var ativo = new RefreshToken { ExpiraEM = agora.AddMinutes(1), Revogado = false };
        var expirado = new RefreshToken { ExpiraEM = agora, Revogado = false };
        var revogado = new RefreshToken { ExpiraEM = agora.AddMinutes(1), Revogado = true };

        Assert.True(ativo.IsActive(agora));
        Assert.False(expirado.IsActive(agora));
        Assert.False(revogado.IsActive(agora));
        Assert.True(expirado.IsExpired(agora));
    }

    [Fact]
    public void IdempotencyRecord_Deve_Nascer_Processing_E_Expirar_Em_24_Horas()
    {
        var agora = new DateTime(2026,8,28,12,0,0,DateTimeKind.Utc);
        var record = new IdempotencyRecord("key",1,"CriarAgendamento","hash",agora);
        Assert.Equal("Processing",record.Status);
        Assert.Equal(agora,record.CreatedAtUtc);
        Assert.Equal(agora.AddHours(24),record.ExpiresAtUtc);
    }

    [Fact]
    public void IdempotencyRecord_Complete_Deve_Registrar_Resposta_E_Data()
    {
        var agora = DateTime.UtcNow;
        var record = new IdempotencyRecord("key",1,"op","hash",agora);
        record.Complete("{\"ok\":true}",agora.AddSeconds(1));
        Assert.Equal("Completed",record.Status);
        Assert.Equal("{\"ok\":true}",record.ResponseBody);
        Assert.Equal(agora.AddSeconds(1),record.CompletedAtUtc);
    }

    [Fact]
    public void IdempotencyRecord_Fail_Deve_Marcar_Failed()
    {
        var record = new IdempotencyRecord("key",1,"op","hash",DateTime.UtcNow);
        record.Fail();
        Assert.Equal("Failed",record.Status);
    }

    [Fact]
    public void OutboxMessage_Deve_Nascer_Nao_Processada_Sem_Retry()
    {
        var id=Guid.NewGuid(); var agora=DateTime.UtcNow;
        var msg=new OutboxMessage(id,"Evento","{}",agora);
        Assert.Equal(id,msg.Id); Assert.Null(msg.ProcessedAtUtc); Assert.Equal(0,msg.RetryCount); Assert.Null(msg.LastError);
    }

    [Fact]
    public void OutboxMessage_MarkProcessed_Deve_Impedir_Processamento_Duplo()
    {
        var msg=new OutboxMessage(Guid.NewGuid(),"Evento","{}",DateTime.UtcNow);
        msg.MarkProcessed(DateTime.UtcNow);
        Assert.NotNull(msg.ProcessedAtUtc);
        Assert.Throws<InvalidOperationException>(()=>msg.MarkProcessed(DateTime.UtcNow));
    }

    [Fact]
    public void OutboxMessage_RegisterFailure_Deve_Incrementar_Retry_E_Truncar_Erro()
    {
        var msg=new OutboxMessage(Guid.NewGuid(),"Evento","{}",DateTime.UtcNow);
        msg.RegisterFailure(new string('x',2500));
        Assert.Equal(1,msg.RetryCount); Assert.Equal(2000,msg.LastError!.Length);
    }

    [Fact]
    public void OutboxMessage_Processada_Nao_Deve_Aceitar_Falha()
    {
        var msg=new OutboxMessage(Guid.NewGuid(),"Evento","{}",DateTime.UtcNow);
        msg.MarkProcessed(DateTime.UtcNow);
        Assert.Throws<InvalidOperationException>(()=>msg.RegisterFailure("erro"));
    }
}

public sealed class InfrastructureStructureTests
{
    [Fact]
    public void Todos_Repositories_E_Queries_Atuais_Devem_Ter_Implementacao()
    {
        var asm=typeof(BarbeariaInfrastructure.Data.AppDbContext).Assembly;
        var names=asm.GetTypes().Select(t=>t.Name).ToHashSet();
        foreach(var required in new[]{
            "UsuarioRepository","AgendamentoRepository","BarbeiroRepository","ServicoRepository","AvaliacaoRepository","RefreshRepository","UnitOfWorksRepository",
            "AgendaDisponibilidadeQuery","BarbeirosQuery","DadosPessoaisQuery","HistoricoClienteQuery","ProximoAgendamentoQuery","ServicosAtivosQuery"})
            Assert.Contains(required,names);
    }

    [Fact]
    public void Classificadores_De_Erro_Dos_Tres_Providers_Devem_Existir()
    {
        var names=typeof(BarbeariaInfrastructure.Data.AppDbContext).Assembly.GetTypes().Select(t=>t.Name).ToHashSet();
        Assert.Contains("PostgreSqlErrorClassifier",names);
        Assert.Contains("MySqlErrorClassifier",names);
        Assert.Contains("SqlServerErrorClassifier",names);
    }

    [Fact]
    public void Implementacoes_De_Repository_Devem_Implementar_Interfaces_Do_Core()
    {
        var asm=typeof(BarbeariaInfrastructure.Data.AppDbContext).Assembly;
        var repos=asm.GetTypes().Where(t=>t.IsClass && t.Namespace?.Contains("Repository")==true && !t.IsAbstract).ToArray();
        foreach(var type in repos.Where(t=>t.Name is not "UnitOfWorksRepository"))
            Assert.NotEmpty(type.GetInterfaces());
    }
}
