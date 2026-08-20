using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Domain.Exceptions;
using BarbeariaCore.Application.Interfaces;

namespace BarbeariaCore.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshRepository _repository;
        private readonly ITokenService _token;
        private readonly ILoginRepository _loginRepository;
        private readonly IUnitOfWork _uow;

        public RefreshTokenService(IRefreshRepository repository, ITokenService token, ILoginRepository loginRepository, IUnitOfWork uow)
        {
            _repository = repository;
            _token = token;
            _loginRepository = loginRepository;
            _uow = uow;
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var token = await _repository.GetAsync(refreshToken);
            if (token is null || token.Revogado)
                throw new DomainException("WRONG_VALUE", "Dados inválidos!");

            await _repository.RevokeAsync(refreshToken, null, "LOGOUT");
            await _uow.SaveChangesAsync();
        }

        public async Task<DTOAuthResponse> GerarRefreshAsync(string refreshToken)
        {
            var refresh = await _repository.GetAsync(refreshToken);
            if (refresh is null)
                throw InvalidRefresh();

            // Um token revogado que já foi substituído indica possível reutilização/roubo.
            if (refresh.Revogado)
            {
                if (!string.IsNullOrWhiteSpace(refresh.ReplacedByToken))
                {
                    await _repository.RevokeFamilyAsync(refresh.FamilyId, "REFRESH_TOKEN_REUSE_DETECTED");
                    await _uow.SaveChangesAsync();
                }
                throw InvalidRefresh();
            }

            if (refresh.EstaExpirado(DateTime.UtcNow))
                throw InvalidRefresh();

            var usuario = await _loginRepository.ObterPorIdAsync(refresh.UsuarioId);
            if (usuario is null || !usuario.Ativado || !usuario.PodeLogar())
                throw new DomainException("AUTH_INVALID_CREDENTIALS", "Credenciais inválidas");

            var accessToken = _token.GenerateToken(usuario);
            var novoRefresh = _token.GenerateRefreshToken();

            await _repository.RevokeAsync(refreshToken, novoRefresh, "ROTATED");
            await _repository.SaveAsync(usuario.Id, novoRefresh, DateTime.UtcNow.AddDays(7), refresh.FamilyId, null);
            await _uow.SaveChangesAsync();

            return new DTOAuthResponse { accessToken = accessToken, refreshToken = novoRefresh };
        }

        public async Task<IReadOnlyList<DTOSessao>> ListarSessoesAsync(int userId, string? currentToken)
        {
            var sessions = await _repository.ListByUserAsync(userId);
            return sessions.Select(x => new DTOSessao
            {
                Id = x.Id,
                CriadoEm = x.CriadoEm,
                ExpiraEm = x.ExpiraEm,
                Revogado = x.Revogado,
                Atual = !string.IsNullOrWhiteSpace(currentToken) && x.Token == currentToken
            }).ToList();
        }

        public async Task RevogarTodasAsync(int userId)
        {
            await _repository.RevokeAllByUserAsync(userId);
            await _uow.SaveChangesAsync();
        }

        public async Task RevogarSessaoAsync(int userId, int sessionId)
        {
            if (!await _repository.RevokeByIdAsync(sessionId, userId))
                throw new DomainException("SESSION_NOT_FOUND", "Sessão não encontrada.");

            await _uow.SaveChangesAsync();
        }

        private static DomainException InvalidRefresh() =>
            new("INVALID_REFRESH", "Credenciais inválidas");
    }
}
