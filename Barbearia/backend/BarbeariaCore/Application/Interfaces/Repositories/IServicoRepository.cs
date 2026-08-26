using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces.Repositories
{
    public interface IServicoRepository
    {
        Task<Servico?> ObterPorIdAsync(int servicoId);
        Task<Servico?> ObterAtivoPorIdAsync(int servicoId);
        Task<bool> ExisteAtivoAsync(int servicoId);

        Task AdicionarAsync(Servico servico);
        Task AtualizarAsync(Servico servico);
    }
}
