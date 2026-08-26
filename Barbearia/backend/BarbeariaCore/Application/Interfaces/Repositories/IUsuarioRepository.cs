using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorIdAsync(int usuarioId);
        Task<Usuario?> ObterPorLoginAsync(string login);
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Usuario?> ObterPorCpfAsync(string cpf);
        Task<Usuario?> ObterPorTelefoneAsync(string telefone);

        Task AdicionarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
    }
}
