using Barbearia.Core.DTO;

namespace Barbearia.Core.Interface
{
    public interface INovoClienteService
    {
        Task<DTOResposta> CadastrarAsync(string nome, string email, string telefone, string cpf, string login, string senha, string foto);
    }
}
