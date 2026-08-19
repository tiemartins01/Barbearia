using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces
{
    public interface ITrocaSenhaService
    {
        Task<DTOResposta> RealizarTrocaSenha(string codigo, string email, string senha, string senharepetida);

    }
}
