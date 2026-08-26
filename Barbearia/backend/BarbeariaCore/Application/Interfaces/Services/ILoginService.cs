using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Services
{
    public interface ILoginService
    {
        Task<DTOAuthResponse> RealizarLoginAsync(string login, string senha);
    }
}
