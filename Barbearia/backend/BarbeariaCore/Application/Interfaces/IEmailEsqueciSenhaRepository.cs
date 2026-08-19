using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces
{
    public interface IEmailEsqueciSenhaRepository
    {
        Task<Usuario?> BuscarUsuarioPorEmailAsync(string email);
        Task AtualizarAsync(Usuario usuario);
    }
}
