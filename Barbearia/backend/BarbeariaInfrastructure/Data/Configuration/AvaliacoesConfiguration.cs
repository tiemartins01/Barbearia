using BarbeariaCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarbeariaCore.Infrastructure.Configuration
{
    public class AvaliacoesConfiguration : IEntityTypeConfiguration<Avaliacao>
    {
        public void Configure(EntityTypeBuilder<Avaliacao> e)
        {
            e.ToTable("comentarios");

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.BarbeiroId).IsRequired().HasColumnName("id_barbeiro");
            e.Property(x => x.ClieteId).IsRequired().HasColumnName("id_cliente");
            e.Property(x => x.AgendamentoId).IsRequired().HasColumnName("id_horario");     
            e.Property(x => x.Nota).IsRequired().HasColumnName("nota");
            e.Property(x => x.Comentario).IsRequired().HasMaxLength(128).HasColumnName("comentario");
            e.Property(x => x.DataAtendimento).IsRequired().HasColumnName("horario");
            e.Property(x => x.ServicoId).IsRequired().HasColumnName("id_servico");

            // RELACIONAMENTO 1:N ONDE UM ÚNICO BARBEIRO PODE TER VÁRIAS AVALIAÇÕES E VÁRIAS AVALIAÇÕES PERTECEREM A UM ÚNICO BARBEIRO
            e.HasOne(x => x.Barbeiro).WithMany(x=> x.Avaliacoes).HasForeignKey(x => x.BarbeiroId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Usuario>().WithMany().HasForeignKey(x => x.ClieteId).OnDelete(DeleteBehavior.Restrict);

            e.HasOne<Agendamento>().WithOne().HasForeignKey<Avaliacao>(x => x.AgendamentoId).OnDelete(DeleteBehavior.Restrict);

            e.HasOne<Servico>().WithMany().HasForeignKey(x => x.ServicoId).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.AgendamentoId).IsUnique().HasDatabaseName("ux_comentarios_horario");
            e.HasIndex(x => new { x.BarbeiroId, x.DataAtendimento }).HasDatabaseName("ix_comentarios_barbeiro_horario");
        }
    }
}
