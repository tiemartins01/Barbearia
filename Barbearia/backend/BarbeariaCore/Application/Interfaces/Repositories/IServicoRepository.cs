using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces.Repositories
{
    public interface IServicoRepository
    {
        Task<Servico?> ObterPorIdAsync(int servicoId, CancellationToken cancellationToken = default);
        Task<Servico?> ObterAtivoPorIdAsync(int servicoId, CancellationToken cancellationToken = default);
        Task<bool> ExisteAtivoAsync(int servicoId, CancellationToken cancellationToken = default);

        Task AdicionarAsync(Servico servico, CancellationToken cancellationToken = default);
        Task AtualizarAsync(Servico servico, CancellationToken cancellationToken = default);
    }
}
