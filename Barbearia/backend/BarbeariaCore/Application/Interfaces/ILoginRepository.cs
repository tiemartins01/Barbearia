using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces
{
    public interface ILoginRepository
    {
        Task<Usuario?> ObterPorLoginAsync(string login);
        Task Atualizar(Usuario usuario);
        Task<Usuario?> ObterPorIdAsync(int id);
    }
}
