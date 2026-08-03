using Barbearia.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Barbearia.Core.Infrastructure.Configuration
{
    public class BarbeiroConfiguration : IEntityTypeConfiguration<Barbeiro>
    {
        public void Configure(EntityTypeBuilder<Barbeiro> e)
        {
            e.ToTable("barbeiro");

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UsuarioId).IsRequired().HasColumnName("usuario_id");
            e.Property(x => x.Especialidade).IsRequired().HasMaxLength(128).HasColumnName("especialidade");
            // RELACIONAMENTO 1:1 ONDE UM ÚNICO BARBEIRO PODE SER APENAS UM USUÁRIO
            e.HasOne(x => x.Usuario).WithOne().HasForeignKey<Barbeiro>(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict); // 1:1
            e.HasIndex(x => x.UsuarioId).IsUnique().HasDatabaseName("ux_barbeiro_usuario");
        }
    }
}
