using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces.Repositories
{
    public interface IAvaliacaoRepository
    {
        Task<Avaliacao?> ObterPorIdAsync(int avaliacaoId, CancellationToken cancellationToken = default);
        Task<bool> ExisteParaAgendamentoAsync(int agendamentoId, CancellationToken cancellationToken = default);
        Task AdicionarAsync(Avaliacao avaliacao, CancellationToken cancellationToken = default);
    }
}
