using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarbeariaInfrastructure.Data.Observability.Audit;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_log");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Action).HasColumnName("action").HasMaxLength(32).IsRequired();
        builder.Property(x => x.EntityType).HasColumnName("entity_type").HasMaxLength(128).IsRequired();
        builder.Property(x => x.EntityId).HasColumnName("entity_id").HasMaxLength(128).IsRequired();
        builder.Property(x => x.OldValues).HasColumnName("old_values");
        builder.Property(x => x.NewValues).HasColumnName("new_values");
        builder.Property(x => x.CorrelationId).HasColumnName("correlation_id").HasMaxLength(128);
        builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(512);
        builder.Property(x => x.RequestPath).HasColumnName("request_path").HasMaxLength(512);
        builder.Property(x => x.RequestMethod).HasColumnName("request_method").HasMaxLength(16);
        builder.HasIndex(x => new { x.UserId, x.OccurredAtUtc }).HasDatabaseName("ix_audit_log_user_occurred");
        builder.HasIndex(x => new { x.EntityType, x.EntityId }).HasDatabaseName("ix_audit_log_entity");
    }
}
