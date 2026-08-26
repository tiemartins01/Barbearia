using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Application.Models;
using BarbeariaCore.Domain.Enum;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Queries
{
    public sealed class AgendaDisponibilidadeQuery : IAgendaDisponibilidadeQuery
    {
        private readonly AppDbContext _context;

        public AgendaDisponibilidadeQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PeriodoOcupado>> BuscarPeriodosOcupadosAsync(
            int barbeiroId,
            DateOnly data)
        {
            var inicioDia = data.ToDateTime(TimeOnly.MinValue);
            var fimDia = data.AddDays(1).ToDateTime(TimeOnly.MinValue);

            return await _context.Agendamentos
                .AsNoTracking()
                .Where(x =>
                    x.BarbeiroId == barbeiroId &&
                    x.Status == StatusAgendamento.Agendado &&
                    x.DataAgendamento < fimDia &&
                    x.HorarioFim > inicioDia)
                .OrderBy(x => x.DataAgendamento)
                .Select(x => new PeriodoOcupado(
                    x.DataAgendamento,
                    x.HorarioFim))
                .ToListAsync();
        }
    }
}
