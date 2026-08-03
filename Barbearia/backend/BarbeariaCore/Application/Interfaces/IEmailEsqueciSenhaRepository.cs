using Barbearia.Core.Domain.Entities;

namespace Barbearia.Core.Interface
{
    public interface IEmailEsqueciSenhaRepository
    {
        Task<Usuario?> BuscarUsuarioPorEmailAsync(string email);
        Task AtualizarAsync(Usuario usuario);
    }
}
