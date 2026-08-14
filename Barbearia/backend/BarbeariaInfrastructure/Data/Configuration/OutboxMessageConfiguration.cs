using Barbearia.Core.Infrastructure.Data.Operational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barbearia.Core.Infrastructure.Configuration;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Type).HasColumnName("type").HasMaxLength(512).IsRequired();
        builder.Property(x => x.Payload).HasColumnName("payload").IsRequired();
        builder.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc");
        builder.Property(x => x.ProcessedAtUtc).HasColumnName("processed_at_utc");
        builder.Property(x => x.RetryCount).HasColumnName("retry_count");
        builder.Property(x => x.LastError).HasColumnName("last_error").HasMaxLength(2000);

        builder.HasIndex(x => new { x.ProcessedAtUtc, x.OccurredAtUtc })
            .HasDatabaseName("ix_outbox_pending");
    }
}
