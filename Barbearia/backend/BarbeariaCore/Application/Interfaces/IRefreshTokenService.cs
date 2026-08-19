using BarbeariaCore.Application.DTOs;


namespace BarbeariaCore.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task RevokeTokenAsync(string RefreshToken);
        Task<DTOAuthResponse> GerarRefreshAsync(string RefreshToken);
        Task<IReadOnlyList<DTOSessao>> ListarSessoesAsync(int userId, string? currentToken);
        Task RevogarTodasAsync(int userId);
        Task RevogarSessaoAsync(int userId, int sessionId);
    }
}
