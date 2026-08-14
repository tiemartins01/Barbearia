using Barbearia.Core.Infrastructure.Data.Operational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barbearia.Core.Infrastructure.Configuration;

public sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("idempotency_records");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Key).HasColumnName("key").HasMaxLength(128).IsRequired();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.Operation).HasColumnName("operation").HasMaxLength(128).IsRequired();
        builder.Property(x => x.RequestHash).HasColumnName("request_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ResponseBody).HasColumnName("response_body").HasColumnType("text");
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(x => x.ExpiresAtUtc).HasColumnName("expires_at_utc");
        builder.Property(x => x.CompletedAtUtc).HasColumnName("completed_at_utc");

        builder.HasIndex(x => new { x.Key, x.UserId, x.Operation })
            .IsUnique()
            .HasDatabaseName("ux_idempotency_key_user_operation");
        builder.HasIndex(x => x.ExpiresAtUtc)
            .HasDatabaseName("ix_idempotency_expires_at");
    }
}
