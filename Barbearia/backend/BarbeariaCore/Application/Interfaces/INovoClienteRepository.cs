using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces
{
    public interface INovoClienteRepository
    {
        Task<Usuario?> VerificarDuplicidadeAsync(string email, string cpf, string telefone, string login);
        Task CadastraNovoClienteAsync(Usuario usuario);
    }
}
