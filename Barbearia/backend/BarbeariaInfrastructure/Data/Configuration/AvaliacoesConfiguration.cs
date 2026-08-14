using Barbearia.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barbearia.Core.Infrastructure.Configuration
{
    public class AvaliacoesConfiguration : IEntityTypeConfiguration<Avaliacoes>
    {
        public void Configure(EntityTypeBuilder<Avaliacoes> e)
        {
            e.ToTable("comentarios");

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.IdBarbeiro).IsRequired().HasColumnName("id_barbeiro");
            e.Property(x => x.IdCliente).IsRequired().HasColumnName("id_cliente");
            e.Property(x => x.IdHorario).IsRequired().HasColumnName("id_horario");     
            e.Property(x => x.Nota).IsRequired().HasColumnName("nota");
            e.Property(x => x.Comentario).IsRequired().HasMaxLength(128).HasColumnName("comentario");
            e.Property(x => x.Horario).IsRequired().HasColumnName("horario");
            e.Property(x => x.IdServico).IsRequired().HasColumnName("id_servico");

            // RELACIONAMENTO 1:N ONDE UM ÚNICO BARBEIRO PODE TER VÁRIAS AVALIAÇÕES E VÁRIAS AVALIAÇÕES PERTECEREM A UM ÚNICO BARBEIRO
            e.HasOne(x => x.BarbeiroF).WithMany(x=> x.Avaliacoes).HasForeignKey(x => x.IdBarbeiro).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Usuario>().WithMany().HasForeignKey(x => x.IdCliente).OnDelete(DeleteBehavior.Restrict);

            e.HasOne<Horarios>().WithOne().HasForeignKey<Avaliacoes>(x => x.IdHorario).OnDelete(DeleteBehavior.Restrict);

            e.HasOne<Servicos>().WithMany().HasForeignKey(x => x.IdServico).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.IdHorario).IsUnique().HasDatabaseName("ux_comentarios_horario");
            e.HasIndex(x => new { x.IdBarbeiro, x.Horario }).HasDatabaseName("ix_comentarios_barbeiro_horario");
        }
    }
}
