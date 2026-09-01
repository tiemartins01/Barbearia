using BarbeariaCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarbeariaCore.Infrastructure.Configuration
{
    public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
    {

        public void Configure(EntityTypeBuilder<Servico> e)
        {
            e.ToTable("servicos");

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nome).IsRequired().HasMaxLength(128).HasColumnName("nome");
            e.Property(x => x.Duracao).IsRequired().HasColumnName("duracao");
            e.Property(x => x.Preco).IsRequired().HasPrecision(10,2).HasColumnName("preco");
            e.Property(x => x.Ativo).IsRequired().HasColumnName("ativo");

            e.HasIndex(x => new { x.Ativo, x.Nome })
                .HasDatabaseName("ix_servicos_ativo_nome");
        }

    }
}
