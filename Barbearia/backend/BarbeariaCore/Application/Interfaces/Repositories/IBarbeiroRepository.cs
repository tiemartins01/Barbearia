using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces.Repositories
{
    public interface IBarbeiroRepository
    {
        Task<Barbeiro?> ObterPorIdAsync(int barbeiroId, CancellationToken cancellationToken = default);
        Task<bool> ExisteAtivoAsync(int barbeiroId, CancellationToken cancellationToken = default);

        Task AdicionarAsync(Barbeiro barbeiro, CancellationToken cancellationToken = default);
        Task AtualizarAsync(Barbeiro barbeiro, CancellationToken cancellationToken = default);
    }
}
