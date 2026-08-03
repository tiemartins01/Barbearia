using Barbearia.Core.Domain.Entities;

namespace Barbearia.Core.Interface
{
    public interface INovoClienteRepository
    {

        Task<Usuario?> VerificarDuplicidadeAsync(string email, string cpf, string telefone, string login);
        Task CadastraNovoClienteAsync(Usuario usuario);
    }
}
