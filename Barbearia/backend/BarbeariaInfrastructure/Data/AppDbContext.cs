using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Domain.Common;
using Barbearia.Core.Infrastructure.Data.Operational;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Barbearia.Core.Application.Abstractions;

namespace Barbearia.Core.Infrastructure.Data
{
    public sealed class AppDbContext : DbContext
    {
        //O AppDbContext não deve decidir sozinho qual banco utilizar.
        //Program.cs decidirá se é PostgreSQL, SQL Server, memória etc.
        private readonly IAuditContext? _auditContext;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IAuditContext? auditContext = null) : base(options)
        {
            _auditContext = auditContext;
        }

        public DbSet<Horarios> Horarios => Set<Horarios>();
        public DbSet<Servicos> Servicos => Set<Servicos>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Avaliacoes> Avaliacoes => Set<Avaliacoes>();
        public DbSet<Barbeiro> Barbeiros => Set<Barbeiro>();
        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddDomainEventsToOutbox();
            AddAuditEntries();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddDomainEventsToOutbox()
        {
            var aggregates = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(entry => entry.Entity.DomainEvents.Count > 0)
                .Select(entry => entry.Entity)
                .ToList();

            var messages = aggregates
                .SelectMany(aggregate => aggregate.DomainEvents)
                .Select(domainEvent => new OutboxMessage(
                    Guid.NewGuid(),
                    domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                    JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    domainEvent.OccurredAtUtc))
                .ToList();

            if (messages.Count > 0)
                OutboxMessages.AddRange(messages);

            foreach (var aggregate in aggregates)
                aggregate.ClearDomainEvents();
        }


        private void AddAuditEntries()
        {
            var excluded = new[] { typeof(AuditLog), typeof(OutboxMessage), typeof(IdempotencyRecord) };
            var entries = ChangeTracker.Entries()
                .Where(e => !excluded.Contains(e.Entity.GetType()) &&
                    e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                var oldValues = new Dictionary<string, object?>();
                var newValues = new Dictionary<string, object?>();

                foreach (var property in entry.Properties)
                {
                    if (IsSensitive(property.Metadata.Name))
                        continue;

                    if (entry.State is EntityState.Modified or EntityState.Deleted)
                        oldValues[property.Metadata.Name] = property.OriginalValue;

                    if (entry.State is EntityState.Added or EntityState.Modified)
                        newValues[property.Metadata.Name] = property.CurrentValue;
                }

                var key = string.Join(",", entry.Properties
                    .Where(p => p.Metadata.IsPrimaryKey())
                    .Select(p => p.CurrentValue?.ToString() ?? string.Empty));

                AuditLogs.Add(new AuditLog(
                    DateTime.UtcNow,
                    _auditContext?.UserId,
                    entry.State.ToString(),
                    entry.Metadata.ClrType.Name,
                    key,
                    oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues),
                    newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues),
                    _auditContext?.CorrelationId,
                    _auditContext?.IpAddress,
                    _auditContext?.UserAgent,
                    _auditContext?.RequestPath,
                    _auditContext?.RequestMethod));
            }
        }

        private static bool IsSensitive(string propertyName)
        {
            var name = propertyName.ToLowerInvariant();
            return name.Contains("senha") || name.Contains("password") || name.Contains("token") ||
                   name.Contains("codigo") || name.Contains("secret");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<Horarios>().Property(x => x.Horario).HasColumnType("timestamp without time zone");
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); // procura as classe que implementam IEntityTypeConfiguration<>
        }
    }
}
