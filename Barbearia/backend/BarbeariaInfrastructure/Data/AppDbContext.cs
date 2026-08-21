using BarbeariaCore.Application.Abstractions;
using BarbeariaCore.Domain.Common;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Infrastructure.Data.Operational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;
using BarbeariaInfrastructure.Security;

namespace BarbeariaCore.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    /*
     * O AppDbContext não decide qual banco será utilizado.
     *
     * Essa decisão é feita na configuração da aplicação:
     *
     * services.AddDbContext<AppDbContext>(options =>
     *     options.UseNpgsql(connectionString));
     *
     * Assim, o contexto recebe as configurações prontas
     * por meio do DbContextOptions.
     */

    // Quem está decidindo não é exatamente o Program.cs, mas a configuração lida pela DatabaseExtensions.
    // Faz a comunicação da aplicação com o banco
    //1. Representar as tabelas do banco por meio dos DbSets.
    //2. Acompanhar entidades adicionadas, alteradas e removidas.
    //3. Converter essas alterações em comandos SQL.
    //4. Antes de salvar

    //Controller
    //    ↓
    //Service
    //    ↓
    //Repository
    //    ↓
    //AppDbContext
    //    ↓
    //ChangeTracker
    //    ↓
    //SaveChangesAsync
    //    ├── cria mensagens de Outbox
    //    ├── cria registros de AuditLog
    //    └── executa INSERT, UPDATE e DELETE
    //            ↓
    //        PostgreSQL

    private readonly IAuditContext? _auditContext;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IAuditContext? auditContext = null)
        : base(options)
    {
        _auditContext = auditContext;
    }

    /*
     * Cada DbSet representa um conjunto de entidades
     * controlado pelo Entity Framework.
     *
     * Normalmente, cada DbSet corresponde a uma tabela.
     */

    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<Servico> Servicos => Set<Servico>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();
    public DbSet<Barbeiro> Barbeiros => Set<Barbeiro>();
    // Eles impedem que uma mesma operação seja executada duas vezes usando a mesma chave.
    public DbSet<IdempotencyRecord> IdempotencyRecords =>
        Set<IdempotencyRecord>();
    // Representa as mensagens da Outbox.
    // Essas mensagens armazenam eventos que deverão ser publicados posteriormente.
    public DbSet<OutboxMessage> OutboxMessages =>
        Set<OutboxMessage>();
    // Representa a tabela de auditoria.
    public DbSet<AuditLog> AuditLogs =>
        Set<AuditLog>();

    /*
     * Versão assíncrona do salvamento.
     *
     * O fluxo é:
     *
     * 1. Captura os Domain Events.
     * 2. Cria as mensagens da Outbox.
     * 3. Captura os dados necessários para auditoria.
     * 4. Salva entidades e Outbox.
     * 5. O banco gera os IDs das entidades novas.
     * 6. Cria os AuditLogs usando os IDs reais.
     * 7. Salva os AuditLogs.
     * 8. Confirma a transação.
     * 9. Limpa os Domain Events.
     */
    // Sobrescrita
    //Se o cliente abandonar a requisição, o token pode interromper o acesso ao banco.
    // Override significa que você substituiu o comportamento padrão do EF Core.

    public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        var aggregatesWithEvents = AddDomainEventsToOutbox();
        var pendingAudits = CreatePendingAuditEntries();

        if (!ChangeTracker.HasChanges())
            return 0;

        // Se já existe uma transação externa,
        // ela continua sendo responsabilidade do chamador.
        if (Database.CurrentTransaction is not null)
        {
            return await SaveInsideExistingTransactionAsync(
                aggregatesWithEvents,
                pendingAudits,
                cancellationToken);
        }

        // Retry strategy do EF/Npgsql
        var strategy = Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var affectedRows =
                    await base.SaveChangesAsync(cancellationToken);

                RefreshGeneratedValues(pendingAudits);

                AddAuditLogs(pendingAudits);

                if (pendingAudits.Count > 0)
                {
                    affectedRows +=
                        await base.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);

                ClearDomainEvents(aggregatesWithEvents);

                return affectedRows;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    private async Task<int> SaveInsideExistingTransactionAsync(
    List<AggregateRoot> aggregatesWithEvents,
    List<PendingAuditEntry> pendingAudits,
    CancellationToken cancellationToken)
    {
        var affectedRows =
            await base.SaveChangesAsync(cancellationToken);

        RefreshGeneratedValues(pendingAudits);

        AddAuditLogs(pendingAudits);

        if (pendingAudits.Count > 0)
        {
            affectedRows +=
                await base.SaveChangesAsync(cancellationToken);
        }

        ClearDomainEvents(aggregatesWithEvents);

        return affectedRows;
    }

//    SaveChangesAsync
//      ↓
//Tem alteração?
//      ↓
//SIM
//      ↓
//Já existe transação externa?
//   ↙              ↘
// SIM NÃO
// ↓                 ↓
//usa ela      CreateExecutionStrategy()
//                   ↓
//              ExecuteAsync()
//                   ↓
//            BeginTransaction
//                   ↓
//              Save #1
//                   ↓
//              AuditLog
//                   ↓
//              Save #2
//                   ↓
//               Commit



    //public override async Task<int> SaveChangesAsync(
    //    CancellationToken cancellationToken = default)
    //{
    //    var aggregatesWithEvents = AddDomainEventsToOutbox();
    //    // Captura as alterações antes de salvar.
    //    // Neste momento ainda não cria os AuditLogs definitivos.
    //    //Isso é necessário porque uma entidade nova pode ainda estar com: id = 0
    //    var pendingAudits = CreatePendingAuditEntries();
    //    /*
    //     * Se o UnitOfWork já iniciou uma transação manual,
    //     * Database.CurrentTransaction não será nulo.
    //     *
    //     * Nesse caso, o AppDbContext utiliza a transação existente
    //     * e não tenta criar outra.
    //     */


    //    /*
    // * Depois de criar possíveis mensagens da Outbox,
    // * verifica se realmente existe algo para salvar.
    // *
    // * Se não houver:
    // * - nenhuma entidade Added;
    // * - nenhuma entidade Modified;
    // * - nenhuma entidade Deleted;
    // * - nenhuma OutboxMessage criada;
    // *
    // * retorna zero sem abrir transação.
    // */
    //    if (!ChangeTracker.HasChanges())
    //        return 0;

    //    if(Database.CurrentTransaction is not null)
    //    {
    //        return await SaveInsideExistingTransactionAsync(
    //        aggregatesWithEvents,
    //        pendingAudits,
    //        cancellationToken);
    //    }

    //    var strategy = Database.CreateExecutionStrategy();

    //    try
    //    {
    //        /*
    //         * Primeiro salvamento:
    //         *
    //         * - entidades de negócio;
    //         * - mensagens da Outbox.
    //         *
    //         * Depois dessa chamada, IDs gerados pelo banco
    //         * já estarão preenchidos nas entidades.
    //         */
    //        // Essa chamada executa o salvamento real do EF Core.
    //        var affectedRows = await base.SaveChangesAsync(
    //            cancellationToken);
    //        // Depois desse salvamento, IDs gerados pelo banco já foram colocados nas entidades.
    //        /*
    //         * Atualiza os valores das propriedades das entidades novas.
    //         *
    //         * Isso é importante porque, antes do primeiro SaveChanges,
    //         * um ID gerado pelo PostgreSQL poderia estar como zero.
    //         */
    //        RefreshGeneratedValues(pendingAudits);

    //        /*
    //         * Agora os AuditLogs são criados usando:
    //         *
    //         * - chave primária real;
    //         * - valores novos atualizados;
    //         * - contexto da requisição.
    //         */
    //        AddAuditLogs(pendingAudits);

    //        if (pendingAudits.Count > 0)
    //        {
    //            affectedRows += await base.SaveChangesAsync(
    //                cancellationToken);
    //        }

    //        /*
    //         * Só confirma aqui quando o próprio AppDbContext
    //         * criou a transação.
    //         *
    //         * Se a transação veio do UnitOfWork, o UnitOfWork
    //         * continua responsável pelo CommitTransactionAsync().
    //         */
    //        if (ownsTransaction && transaction is not null)
    //        {
    //            await transaction.CommitAsync(cancellationToken);
    //        }

    //        /*
    //         * Os eventos só são removidos depois que todos os
    //         * salvamentos foram concluídos com sucesso.
    //         */
    //        ClearDomainEvents(aggregatesWithEvents);

    //        return affectedRows;
    //    }
    //    catch
    //    {
    //        /*
    //         * Só executa o rollback quando esta classe
    //         * criou a transação.
    //         *
    //         * Se a transação foi iniciada pelo UnitOfWork,
    //         * o fluxo externo deverá executar RollbackAsync().
    //         */
    //        if (ownsTransaction && transaction is not null)
    //        {
    //            await transaction.RollbackAsync(cancellationToken);
    //        }

    //        /*
    //         * Não limpa os DomainEvents.
    //         * A exceção continua subindo até o middleware.
    //         */
    //        throw;
    //    }
    //    finally
    //    {
    //        if (transaction is not null)
    //        {
    //            await transaction.DisposeAsync();
    //        }
    //    }
    //}

    /*
     * Versão síncrona.
     *
     * Ela existe para impedir que uma chamada acidental a:
     *
     * _context.SaveChanges()
     *
     * ignore a Outbox e a auditoria.
     *
     * Apesar disso, no projeto, prefira SaveChangesAsync().
     */
    public override int SaveChanges()
    {
        var aggregatesWithEvents = AddDomainEventsToOutbox();
        var pendingAudits = CreatePendingAuditEntries();

        var ownsTransaction = Database.CurrentTransaction is null;
        IDbContextTransaction? transaction = null;

        if (ownsTransaction)
        {
            transaction = Database.BeginTransaction();
        }

        try
        {
            var affectedRows = base.SaveChanges();

            RefreshGeneratedValues(pendingAudits);
            AddAuditLogs(pendingAudits);

            if (pendingAudits.Count > 0)
            {
                affectedRows += base.SaveChanges();
            }

            if (ownsTransaction && transaction is not null)
            {
                transaction.Commit();
            }

            ClearDomainEvents(aggregatesWithEvents);

            return affectedRows;
        }
        catch
        {
            if (ownsTransaction && transaction is not null)
            {
                transaction.Rollback();
            }

            throw;
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    /*
     * Procura entidades AggregateRoot que possuem
     * Domain Events pendentes.
     *
     * Cada evento vira uma OutboxMessage.
     *
     * O método retorna os aggregates encontrados para
     * que os eventos sejam limpos somente após o sucesso.
     */

    // Antes de salvar, procura eventos de domínio pendentes e os transforma em OutboxMessage
    // Tentar impedir que uma ação importante seja perdida
    private List<AggregateRoot> AddDomainEventsToOutbox()
    {
        var aggregates = ChangeTracker // acessa o ratreador do EF. O ChangeTracker acompanha as entidades carregadas ou adicionadas ao contexto.
            .Entries<AggregateRoot>() // Apenas quem herda de AggregateRoot
            .Where(entry => entry.Entity.DomainEvents.Count > 0) // Filtra apenas entidades que possuem eventos pendentes.
            .Select(entry => entry.Entity)
            .ToList();


        var messages = aggregates
                .SelectMany(aggregate => aggregate.DomainEvents) // Cada aggregate pode ter vários eventos. O SelectMany transforma várias listas em uma única sequência
                .Select(domainEvent => new OutboxMessage( // Converte cada DomainEvent em uma entidade OutboxMessage.
                    Guid.NewGuid(), // Gera um identificador único para a mensagem.
                    domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                    JsonSerializer.Serialize(domainEvent, domainEvent.GetType()), // Transforma o evento em JSON.
                    domainEvent.OccurredAtUtc)) // Guarda quando o evento de domínio aconteceu.
                .ToList();

        //Exemplo:

        //Usuario
        //├── SenhaAlteradaDomainEvent
        //└── UsuarioAtivacaoAlteradaDomainEvent

        // Adiciona mensagens se tiver alguma

        if (messages.Count > 0)
        {
            OutboxMessages.AddRange(messages);
        }

        return aggregates;
    }

    /*
     * Remove os Domain Events da memória.
     *
     * Esse método só é chamado depois que o banco
     * confirmou todos os salvamentos.
     */
    private static void ClearDomainEvents(
        IEnumerable<AggregateRoot> aggregates)
    {
        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }
    }

    /*
     * Captura as alterações antes do primeiro SaveChanges.
     *
     * Nesse momento, os AuditLogs ainda não são adicionados
     * ao DbContext.
     *
     * Eles ficam temporariamente representados por
     * PendingAuditEntry.
     */
    private List<PendingAuditEntry> CreatePendingAuditEntries()
    {
        var excludedTypes = new[]
        {
            typeof(AuditLog),
            typeof(OutboxMessage),
            typeof(IdempotencyRecord)
        };

        var entries = ChangeTracker
            .Entries()
            .Where(entry =>
                !excludedTypes.Contains(
                    entry.Entity.GetType()) &&
                entry.State is
                    EntityState.Added or
                    EntityState.Modified or
                    EntityState.Deleted)
            .ToList();

        var pendingAudits =
            new List<PendingAuditEntry>();

        foreach (var entry in entries)
        {
            var pendingAudit = new PendingAuditEntry
            {
                Entry = entry,
                Action = entry.State.ToString(),
                EntityName = entry.Metadata.ClrType.Name
            };

            foreach (var property in entry.Properties)
            {
                var propertyName =
                    property.Metadata.Name;

                if (IsSensitive(propertyName))
                {
                    continue;
                }

                /*
                 * Em uma alteração, registra somente propriedades
                 * que realmente foram modificadas.
                 *
                 * Isso evita salvar novamente todas as colunas
                 * da entidade no AuditLog.
                 */
                if (entry.State == EntityState.Modified)
                {
                    if (!property.IsModified)
                    {
                        continue;
                    }

                    pendingAudit.OldValues[propertyName] =
                        property.OriginalValue;

                    pendingAudit.NewValues[propertyName] =
                        property.CurrentValue;

                    continue;
                }

                /*
                 * Em uma exclusão, guarda os dados anteriores,
                 * pois a entidade deixará de existir.
                 */
                if (entry.State == EntityState.Deleted)
                {
                    pendingAudit.OldValues[propertyName] =
                        property.OriginalValue;

                    continue;
                }

                /*
                 * Em uma inclusão, todos os valores são novos.
                 *
                 * Alguns deles, como IDs gerados pelo banco,
                 * serão atualizados após o primeiro SaveChanges.
                 */
                if (entry.State == EntityState.Added)
                {
                    pendingAudit.NewValues[propertyName] =
                        property.CurrentValue;
                }
            }

            pendingAudits.Add(pendingAudit);
        }

        return pendingAudits;
    }

    /*
     * Depois do primeiro SaveChanges, o PostgreSQL pode ter
     * gerado valores como o ID da entidade.
     *
     * Este método atualiza os valores novos da auditoria
     * para entidades que foram adicionadas.
     */

    //Fluxo:

    //Primeiro SaveChanges
    //→ entidades + Outbox

    //Segundo SaveChanges
    //→ AuditLogs
    private static void RefreshGeneratedValues( // Atualiza os valores da auditoria pendente.
        IEnumerable<PendingAuditEntry> pendingAudits)
    {
        foreach (var pendingAudit in pendingAudits)
        {
            if (pendingAudit.Action !=
                EntityState.Added.ToString())
            {
                continue;
            }

            foreach (var property in
                     pendingAudit.Entry.Properties)
            {
                var propertyName =
                    property.Metadata.Name;

                if (IsSensitive(propertyName))
                {
                    continue;
                }

                pendingAudit.NewValues[propertyName] =
                    property.CurrentValue;
            }
        }
    }

    /*
     * Cria os registros definitivos de AuditLog.
     *
     * Nesse momento, o primeiro SaveChanges já ocorreu,
     * portanto IDs gerados pelo banco já estão disponíveis.
     */
    private void AddAuditLogs(
        IEnumerable<PendingAuditEntry> pendingAudits)
    {
        foreach (var pendingAudit in pendingAudits)
        {
            var entityKey = string.Join(
                ",",
                pendingAudit.Entry.Properties
                    .Where(property =>
                        property.Metadata.IsPrimaryKey())
                    .Select(property =>
                        property.CurrentValue?.ToString()
                        ?? string.Empty));

            /*
             * Quando não existe requisição HTTP, por exemplo:
             *
             * - BackgroundService;
             * - processo interno;
             * - rotina administrativa;
             *
             * o AuditContext pode não possuir usuário.
             *
             * Como UserId é int?, null representa uma
             * operação interna do sistema.
             */
            AuditLogs.Add(new AuditLog(
                DateTime.UtcNow,
                _auditContext?.UserId,
                pendingAudit.Action,
                pendingAudit.EntityName,
                entityKey,
                SerializeOrNull(
                    pendingAudit.OldValues),
                SerializeOrNull(
                    pendingAudit.NewValues),
                _auditContext?.CorrelationId,
                _auditContext?.IpAddress,
                _auditContext?.UserAgent,
                _auditContext?.RequestPath,
                _auditContext?.RequestMethod));
        }
    }

    /*
     * Evita gravar um JSON vazio na tabela.
     */
    private static string? SerializeOrNull(
        Dictionary<string, object?> values)
    {
        return values.Count == 0
            ? null
            : JsonSerializer.Serialize(values);
    }

    /*
     * Impede que dados sensíveis sejam gravados
     * na tabela de auditoria.
     */
    private static bool IsSensitive(
        string propertyName)
    {
        var name =
            propertyName.ToLowerInvariant();

        return name.Contains("senha") ||
               name.Contains("password") ||
               name.Contains("token") ||
               name.Contains("codigo") ||
               name.Contains("Credential") ||
               name.Contains("Pin") ||
               name.Contains("secret");
    }

    /*
     * Configura o modelo do Entity Framework.
     *
     * ApplyConfigurationsFromAssembly procura automaticamente
     * classes que implementam:
     *
     * IEntityTypeConfiguration<T>
     *
     * Exemplo:
     *
     * UsuarioConfiguration
     * AuditLogConfiguration
     * OutboxMessageConfiguration
     */
    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }

    /*
     * Estrutura temporária utilizada para guardar
     * informações de auditoria entre os dois salvamentos.
     *
     * Ela não é uma entidade e não vira tabela.
     */
    private sealed class PendingAuditEntry
    {
        public required EntityEntry Entry { get; init; }

        public required string Action { get; init; }

        public required string EntityName { get; init; }

        public Dictionary<string, object?> OldValues { get; } =
            new();

        public Dictionary<string, object?> NewValues { get; } =
            new();
    }
}

