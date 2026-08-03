using Barbearia.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barbearia.Core.Infrastructure.Configuration
{
    public class RefreshConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_token");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Id_usuario).IsRequired().HasColumnName("usuario_id");
            builder.Property(x => x.Token).IsRequired().HasMaxLength(512).HasColumnName("token");
            builder.Property(x => x.ExpiraEM).IsRequired().HasColumnName("expira_em").HasColumnType("timestamp with time zone");
            builder.Property(x => x.CriadoEM).IsRequired().HasColumnName("criado_em").HasColumnType("timestamp with time zone");
            builder.Property(x => x.Revogado).IsRequired().HasColumnName("revogado");
            builder.Property(x => x.FamilyId).IsRequired().HasColumnName("family_id");
            builder.Property(x => x.ReplacedByToken).HasMaxLength(512).HasColumnName("replaced_by_token");
            builder.Property(x => x.RevokedAtUtc).HasColumnName("revoked_at_utc").HasColumnType("timestamp with time zone");
            builder.Property(x => x.RevocationReason).HasMaxLength(128).HasColumnName("revocation_reason");
            builder.Property(x => x.CreatedByIp).HasMaxLength(64).HasColumnName("created_by_ip");

            builder.HasIndex(x => x.Token).IsUnique().HasDatabaseName("ux_refresh_token_token");
            builder.HasIndex(x => x.FamilyId).HasDatabaseName("ix_refresh_token_family");
            builder.HasIndex(x => new { x.Id_usuario, x.Revogado, x.ExpiraEM })
                .HasDatabaseName("ix_refresh_token_usuario_revogado_expira");
            builder.HasOne<Usuario>().WithMany().HasForeignKey(x => x.Id_usuario).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
