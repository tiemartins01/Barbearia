using Barbearia.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Barbearia.Core.Infrastructure.Configuration
{
    public class HorariosConfiguration : IEntityTypeConfiguration<Horarios>
    {

        public void Configure(EntityTypeBuilder<Horarios> e)
        {
            e.ToTable("horarios");

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Id_barbeiro).IsRequired().HasColumnName("id_barbeiro");
            e.Property(x => x.Id_cliente).IsRequired().HasColumnName("id_cliente");
            e.Property(x => x.Horario).IsRequired().HasColumnName("horario");
            e.Property(x => x.StatusAgendamento).IsRequired().HasColumnName("status");
            e.Property(x => x.Id_servico).IsRequired().HasColumnName("id_servico");


            // RELACIONAMENTO 1:N ONDE UM ÚNICO BARBEIRO PODE TER VÁRIOS HORÁRIOS E VÁRIAS HORÁRIOS PERTECEREM A UM ÚNICO BARBEIRO
            e.HasOne(x => x.Barbeiro).WithMany().HasForeignKey(x => x.Id_barbeiro).OnDelete(DeleteBehavior.Restrict);
            // RELACIONAMENTO 1:N ONDE UM ÚNICO CLIENTE PODE TER VÁRIOS HORÁRIOS E VÁRIAS HORÁRIOS PERTECEREM A UM ÚNICO CLIENTE
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.Id_cliente).OnDelete(DeleteBehavior.Restrict);
            // RELACIONAMENTO 1:N ONDE UM ÚNICO SERVIÇO PODE ESTAR EM VÁRIOS HORÁRIOS E VÁRIAS HORÁRIOS TEREM O MESMO SERVIÇO
            e.HasOne(x => x.Servicos).WithMany().HasForeignKey(x => x.Id_servico).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.Id_cliente, x.StatusAgendamento, x.Horario })
                .HasDatabaseName("ix_horarios_cliente_status_horario");

            e.HasIndex(x => new { x.Id_barbeiro, x.Horario, x.StatusAgendamento })
                .HasDatabaseName("ix_horarios_barbeiro_horario_status");

            //e.HasIndex(x => new { x.Id_barbeiro, x.Horario })
            //    .IsUnique()
            //    .HasDatabaseName("ux_horarios_barbeiro_horario_ativo"); // .HasFilter("\"status\" = 0")
        }

    }
}
