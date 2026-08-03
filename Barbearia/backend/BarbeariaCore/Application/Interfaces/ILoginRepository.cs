using Barbearia.Core.Domain.Entities;

namespace Barbearia.Core.Interface
{
    public interface ILoginRepository
    {
        Task<Usuario?> ObterPorLoginAsync(string login);
        Task Atualizar(Usuario usuario);
        Task<Usuario?> ObterPorIdAsync(int id);
    }
}
