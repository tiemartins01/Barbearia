using BarbeariaCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BarbeariaCore.Infrastructure.Configuration
{
    public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
    {
        public void Configure(EntityTypeBuilder<Agendamento> e)
        {
            e.ToTable("agendamentos");

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.BarbeiroId).IsRequired().HasColumnName("id_barbeiro");
            e.Property(x => x.ClienteId).IsRequired().HasColumnName("id_cliente");
            e.Property(x => x.DataAgendamento).IsRequired().HasColumnName("horario");
            e.Property(x => x.Status).IsRequired().HasColumnName("status");
            e.Property(x => x.ServicoId).IsRequired().HasColumnName("id_servico");
            e.Property(x => x.DuracaoMinutos).IsRequired().HasColumnName("duracaoMinutos");
            e.Property(x => x.HorarioFim).IsRequired().HasColumnName("horariofim");


            // RELACIONAMENTO 1:N ONDE UM ÚNICO BARBEIRO PODE TER VÁRIOS HORÁRIOS E VÁRIAS HORÁRIOS PERTECEREM A UM ÚNICO BARBEIRO
            e.HasOne(x => x.Barbeiro).WithMany().HasForeignKey(x => x.BarbeiroId).OnDelete(DeleteBehavior.Restrict);
            // RELACIONAMENTO 1:N ONDE UM ÚNICO CLIENTE PODE TER VÁRIOS HORÁRIOS E VÁRIAS HORÁRIOS PERTECEREM A UM ÚNICO CLIENTE
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
            // RELACIONAMENTO 1:N ONDE UM ÚNICO SERVIÇO PODE ESTAR EM VÁRIOS HORÁRIOS E VÁRIAS HORÁRIOS TEREM O MESMO SERVIÇO
            e.HasOne(x => x.Servico).WithMany().HasForeignKey(x => x.ServicoId).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.ClienteId, x.Status, x.DataAgendamento })
                .HasDatabaseName("ix_horarios_cliente_status_horario");

            e.HasIndex(x => new { x.BarbeiroId, x.DataAgendamento, x.Status })
                .HasDatabaseName("ix_horarios_barbeiro_horario_status");

            //e.HasIndex(x => new { x.Id_barbeiro, x.Horario })
            //    .IsUnique()
            //    .HasDatabaseName("ux_horarios_barbeiro_horario_ativo"); // .HasFilter("\"status\" = 0")
        }

    }
}
