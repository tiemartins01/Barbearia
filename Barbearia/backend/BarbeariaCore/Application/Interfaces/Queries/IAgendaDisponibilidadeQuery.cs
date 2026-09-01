using BarbeariaCore.Application.Models;

namespace BarbeariaCore.Application.Interfaces.Queries
{
    public interface IAgendaDisponibilidadeQuery
    {
        Task<IReadOnlyList<PeriodoOcupado>> BuscarPeriodosOcupadosAsync(
            int barbeiroId,
            DateOnly data,
            CancellationToken cancellationToken);
    }
}
