using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorIdAsync(int usuarioId, CancellationToken cancellationToken = default);
        Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken cancellationToken = default);
        Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Usuario?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default);
        Task<Usuario?> ObterPorTelefoneAsync(string telefone, CancellationToken cancellationToken = default);
        Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default);
        Task AtualizarAsync(Usuario usuario, CancellationToken cancellationToken = default);
    }
}
