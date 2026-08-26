using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces.Repositories
{
    public interface IAvaliacaoRepository
    {
        Task<Avaliacao?> ObterPorIdAsync(int avaliacaoId);
        Task<bool> ExisteParaAgendamentoAsync(int agendamentoId);
        Task AdicionarAsync(Avaliacao avaliacao);
    }
}
