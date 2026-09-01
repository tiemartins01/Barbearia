using BarbeariaCore.Application.Models;
using System.Threading;

namespace BarbeariaCore.Application.Interfaces
{
    public interface IRefreshRepository
    {
        Task SaveAsync(int usuarioId, string refreshToken, DateTime expiraEm, CancellationToken cancellationToken = default);
        Task SaveAsync(int usuarioId, string refreshToken, DateTime expiraEm, Guid familyId, string? createdByIp
            , CancellationToken cancellationToken = default);
        Task<RefreshTokenData?> GetAsync(string token, CancellationToken cancellationToken = default);
        Task RevokeAsync(string token, CancellationToken cancellationToken = default);
        Task RevokeAsync(string token, string? replacedByToken, string? reason
            , CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RefreshTokenData>> ListByUserAsync(int userId
            , CancellationToken cancellationToken = default);
        Task RevokeAllByUserAsync(int userId, CancellationToken cancellationToken = default);
        Task RevokeFamilyAsync(Guid familyId, string reason,CancellationToken cancellationToken = default);
        Task<bool> RevokeByIdAsync(int sessionId, int userId, CancellationToken cancellationToken = default );
    }
}
