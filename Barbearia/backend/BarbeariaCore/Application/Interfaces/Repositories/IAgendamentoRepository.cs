using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces.Repositories
{
    public interface IAgendamentoRepository
    {
        Task<Agendamento?> ObterPorIdAsync(int agendamentoId, CancellationToken cancellationToken = default);

        Task<bool> ExisteConflitoAsync(
            int barbeiroId,
            DateTime inicio,
            DateTime fim,
            CancellationToken cancellationToken = default);

        Task AdicionarAsync(Agendamento agendamento,CancellationToken cancellationToken = default);
        Task AtualizarAsync(Agendamento agendamento, CancellationToken cancellationToken = default);
    }
}
