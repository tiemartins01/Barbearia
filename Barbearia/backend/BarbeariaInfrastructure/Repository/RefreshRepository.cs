using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Infrastructure.Data;
using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public class RefreshRepository : IRefreshRepository
    {
        private readonly AppDbContext _context;

        public RefreshRepository(AppDbContext context) => _context = context;

        public Task SaveAsync(int usuarioId, string refreshToken, DateTime expiraEm) =>
            SaveAsync(usuarioId, refreshToken, expiraEm, Guid.NewGuid(), null);

        public async Task SaveAsync(int usuarioId, string refreshToken, DateTime expiraEm, Guid familyId, string? createdByIp)
        {
            var token = new RefreshToken
            {
                Id_usuario = usuarioId,
                Token = refreshToken,
                ExpiraEM = ToUtc(expiraEm),
                CriadoEM = DateTime.UtcNow,
                Revogado = false,
                FamilyId = familyId,
                CreatedByIp = createdByIp
            };

            await _context.RefreshTokens.AddAsync(token);
        }

        public Task<RefreshToken?> GetAsync(string token) =>
            _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == token);

        public Task RevokeAsync(string token) => RevokeAsync(token, null, null);

        public async Task RevokeAsync(string token, string? replacedByToken, string? reason)
        {
            var refresh = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
            if (refresh is null) return;

            refresh.Revogado = true;
            refresh.RevokedAtUtc = DateTime.UtcNow;
            refresh.ReplacedByToken = replacedByToken;
            refresh.RevocationReason = reason;
        }

        public async Task<IReadOnlyList<RefreshToken>> ListByUserAsync(int userId) =>
            await _context.RefreshTokens.AsNoTracking()
                .Where(x => x.Id_usuario == userId)
                .OrderByDescending(x => x.CriadoEM)
                .ToListAsync();

        public async Task RevokeAllByUserAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(x => x.Id_usuario == userId && !x.Revogado)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revogado = true;
                token.RevokedAtUtc = DateTime.UtcNow;
                token.RevocationReason = "REVOKE_ALL_SESSIONS";
            }
        }

        public async Task RevokeFamilyAsync(Guid familyId, string reason)
        {
            var tokens = await _context.RefreshTokens
                .Where(x => x.FamilyId == familyId && !x.Revogado)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revogado = true;
                token.RevokedAtUtc = DateTime.UtcNow;
                token.RevocationReason = reason;
            }
        }

        public async Task<bool> RevokeByIdAsync(int sessionId, int userId)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Id == sessionId && x.Id_usuario == userId);

            if (token is null) return false;
            if (!token.Revogado)
            {
                token.Revogado = true;
                token.RevokedAtUtc = DateTime.UtcNow;
                token.RevocationReason = "USER_REVOKED_SESSION";
            }
            return true;
        }
        private static DateTime ToUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

    }
}
