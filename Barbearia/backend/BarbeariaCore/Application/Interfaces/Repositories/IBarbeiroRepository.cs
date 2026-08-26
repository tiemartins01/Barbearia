using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces.Repositories
{
    public interface IBarbeiroRepository
    {
        Task<Barbeiro?> ObterPorIdAsync(int barbeiroId);
        Task<bool> ExisteAtivoAsync(int barbeiroId);

        Task AdicionarAsync(Barbeiro barbeiro);
        Task AtualizarAsync(Barbeiro barbeiro);
    }
}
