using Barbearia.Core.DTO;

namespace Barbearia.Core.Interface
{
    public interface ITrocaSenhaService
    {
        Task<DTOResposta> RealizarTrocaSenha(string codigo, string email, string senha, string senharepetida);

    }
}
