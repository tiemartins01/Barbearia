using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces
{
    public interface INovoClienteService
    {
        Task<DTOResposta> CadastrarAsync(string nome, string email, string telefone, string cpf, string login, string senha, string foto);
    }
}
