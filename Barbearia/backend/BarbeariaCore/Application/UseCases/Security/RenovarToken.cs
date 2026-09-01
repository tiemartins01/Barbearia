using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;

namespace BarbeariaCore.UseCases.Security
{
    public sealed class RenovarToken
    {
        private readonly IRefreshRepository _repository;
        private readonly ITokenService _token;
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _uow;


        public RenovarToken(IRefreshRepository repository, ITokenService token,
            IUsuarioRepository usuarios, IUnitOfWork uow)
        {
            _uow = uow;
            _repository = repository;   
            _token = token;
            _usuarios = usuarios;
        }

        public async Task<DTOAuthResponse> ExecutarAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var agora = DateTime.UtcNow;

            var refresh = await _repository.GetAsync(refreshToken, cancellationToken);
            if (refresh is null)
                throw InvalidRefresh();

            // Um token revogado que já foi substituído indica possível reutilização/roubo.
            if (refresh.Revogado)
            {
                if (!string.IsNullOrWhiteSpace(refresh.ReplacedByToken))
                {
                    await _repository.RevokeFamilyAsync(refresh.FamilyId, "REFRESH_TOKEN_REUSE_DETECTED", cancellationToken);
                    await _uow.SaveChangesAsync(cancellationToken);
                }
                throw InvalidRefresh();
            }

            if (refresh.EstaExpirado(DateTime.UtcNow))
                throw InvalidRefresh();

            var usuario = await _usuarios.ObterPorIdAsync(refresh.UsuarioId, cancellationToken);
            if (usuario is null || !usuario.Ativado || !usuario.PodeAutenticar(agora))
                throw new AuthenticationException("AUTH_INVALID_CREDENTIALS", "Credencial inválida");

            var accessToken = _token.GenerateToken(usuario);
            var novoRefresh = _token.GenerateRefreshToken();

            await _repository.RevokeAsync(refreshToken, novoRefresh, "ROTATED", cancellationToken);
            await _repository.SaveAsync(usuario.Id, novoRefresh, DateTime.UtcNow.AddDays(7), refresh.FamilyId, null,cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return new DTOAuthResponse { AccessToken = accessToken, RefreshToken = novoRefresh };
        }
        private static AuthenticationException InvalidRefresh() =>
            new("INVALID_REFRESH", "Credenciais inválidas");
    }
}
