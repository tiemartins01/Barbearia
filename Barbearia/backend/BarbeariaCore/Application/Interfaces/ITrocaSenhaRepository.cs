
using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces
{
    public interface ITrocaSenhaRepository
    {
        Task<Usuario?> PegaInformacaoUsuario(string email);
        Task AtualizaUsuario(Usuario usuario);
    }
}
