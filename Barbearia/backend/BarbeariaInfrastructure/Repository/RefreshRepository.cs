using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Models;
using BarbeariaInfrastructure.Data;
using BarbeariaInfrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public class RefreshRepository : IRefreshRepository
    {
        private readonly AppDbContext _context;

        public RefreshRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task SaveAsync(
            int usuarioId,
            string refreshToken,
            DateTime expiraEm, CancellationToken cancellationToken = default)
        {
            return SaveAsync(
                usuarioId,
                refreshToken,
                expiraEm,
                Guid.NewGuid(),
                null,
                cancellationToken);
        }

        public async Task SaveAsync(
            int usuarioId,
            string refreshToken,
            DateTime expiraEm,
            Guid familyId,
            string? createdByIp, CancellationToken cancellationToken = default)
        {
            var token = new RefreshToken(
                usuarioId,
                refreshToken,
                ToUtc(expiraEm),
                familyId,
                createdByIp,
                DateTime.UtcNow
            );

            await _context.RefreshTokens.AddAsync(token, cancellationToken);
        }

        public async Task<RefreshTokenData?> GetAsync(string token,CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .Where(x => x.Token == token)
                .Select(x => new RefreshTokenData
                {
                    Id = x.Id,
                    UsuarioId = x.UsuarioId,
                    Token = x.Token,
                    ExpiraEm = x.ExpiraEm,
                    CriadoEm = x.CriadoEm,
                    Revogado = x.Revogado,
                    FamilyId = x.FamilyId,
                    CreatedByIp = x.CreatedByIp,
                    ReplacedByToken = x.ReplacedByToken,
                    RevocationReason = x.RevocationReason
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task RevokeAsync(string token, CancellationToken cancellationToken = default)
        {
            return RevokeAsync(token, null, null, cancellationToken);
        }

        public async Task RevokeAsync(
            string token,
            string? replacedByToken,
            string? reason,
            CancellationToken cancellationToken = default)
        {
            var refresh = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

            if (refresh is null)
                return;

            refresh.Revogar(DateTime.UtcNow,
            replacedByToken,
            reason);
        }

        public async Task<IReadOnlyList<RefreshTokenData>>
            ListByUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .Where(x => x.UsuarioId == userId)
                .OrderByDescending(x => x.CriadoEm)
                .Select(x => new RefreshTokenData
                {
                    Id = x.Id,
                    UsuarioId = x.UsuarioId,
                    Token = x.Token,
                    ExpiraEm = x.ExpiraEm,
                    CriadoEm = x.CriadoEm,
                    Revogado = x.Revogado,
                    FamilyId = x.FamilyId,
                    CreatedByIp = x.CreatedByIp,
                    ReplacedByToken = x.ReplacedByToken,
                    RevocationReason = x.RevocationReason
                })
                .ToListAsync(cancellationToken);
        }

        public async Task RevokeAllByUserAsync(int userId, CancellationToken cancellationToken)
        {
            var tokens = await _context.RefreshTokens
                .Where(x =>
                    x.UsuarioId == userId &&
                    !x.Revogado)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens)
            {
                token.Revogar(
                    DateTime.UtcNow,
                    null,
                    "REVOKE_ALL_SESSIONS");
            }
        }

        public async Task RevokeFamilyAsync(
            Guid familyId,
            string reason,
            CancellationToken cancellationToken)
        {
            var tokens = await _context.RefreshTokens
                .Where(x =>
                    x.FamilyId == familyId &&
                    !x.Revogado)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens)
            {
                token.Revogar(
                    DateTime.UtcNow,
                    null,
                    reason);
            }
        }

        public async Task<bool> RevokeByIdAsync(
            int sessionId,
            int userId,
            CancellationToken cancellationToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.Id == sessionId &&
                    x.UsuarioId == userId, cancellationToken);

            if (token is null)
                return false;

            if (!token.Revogado)
            {
                token.Revogar(
                DateTime.UtcNow,
                null,
                "USER_REVOKED_SESSION");
            }

            return true;
        }

        private static DateTime ToUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,

                DateTimeKind.Local =>
                    value.ToUniversalTime(),

                _ => DateTime.SpecifyKind(
                    value,
                    DateTimeKind.Utc)
            };
        }
    }
}