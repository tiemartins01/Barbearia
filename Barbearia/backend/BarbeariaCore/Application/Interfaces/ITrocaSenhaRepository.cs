
using Barbearia.Core.Domain.Entities;

namespace Barbearia.Core.Interface
{
    public interface ITrocaSenhaRepository
    {
        Task<Usuario?> PegaInformacaoUsuario(string email);
        Task AtualizaUsuario(Usuario usuario);
    }
}
