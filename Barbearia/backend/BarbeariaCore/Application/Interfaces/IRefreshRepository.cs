using BarbeariaCore.Application.Models;

namespace BarbeariaCore.Application.Interfaces
{
    public interface IRefreshRepository
    {
        Task SaveAsync(int usuarioId, string refreshToken, DateTime expiraEm);
        Task SaveAsync(int usuarioId, string refreshToken, DateTime expiraEm, Guid familyId, string? createdByIp);
        Task<RefreshTokenData?> GetAsync(string token);
        Task RevokeAsync(string token);
        Task RevokeAsync(string token, string? replacedByToken, string? reason);
        Task<IReadOnlyList<RefreshTokenData>> ListByUserAsync(int userId);
        Task RevokeAllByUserAsync(int userId);
        Task RevokeFamilyAsync(Guid familyId, string reason);
        Task<bool> RevokeByIdAsync(int sessionId, int userId);
    }
}
