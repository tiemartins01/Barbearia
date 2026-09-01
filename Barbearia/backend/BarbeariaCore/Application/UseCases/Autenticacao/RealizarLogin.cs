using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;

namespace BarbeariaCore.UseCases.Autenticacao
{
    public sealed class RealizarLogin
    {

        private readonly IUsuarioRepository _usuarios;
        private readonly ITokenService _token;
        private readonly IRefreshRepository _refresh;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<RealizarLogin> _logger;
        private readonly IPasswordHash _passwordHash;

        public RealizarLogin(
            IUsuarioRepository usuarios,
            ITokenService token,
            IRefreshRepository refresh,
            IUnitOfWork uow,
            ILogger<RealizarLogin> logger,
            IPasswordHash passwordHash)
        {
            _usuarios = usuarios;
            _token = token;
            _refresh = refresh;
            _uow = uow;
            _logger = logger;
            _passwordHash = passwordHash;
        }

        public async Task<DTOAuthResponse> ExecutarAsync(string login, string senha, CancellationToken cancellationToken)
        {
            login = login.Trim().ToLowerInvariant();
            var agora = DateTime.UtcNow;

            var usuario = await _usuarios.ObterPorLoginAsync(login, cancellationToken);

            if (usuario is null)
            {
                _logger.LogWarning("Tentativa de acessar com usuário inexistente: {Login}", login);
                throw CredenciaisInvalidas();
            }

            if (!usuario.PodeAutenticar(agora))
            {
                _logger.LogWarning("Tentativa de autenticação inválida para {Login}", login);
                throw CredenciaisInvalidas();
            }

            if (!_passwordHash.Verify(senha, usuario.Senha.Hash))
            {
                usuario.RegistrarFalhaLogin(agora);
                await _usuarios.AtualizarAsync(usuario, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogWarning("Senha incorreta para {Login}", login);
                throw CredenciaisInvalidas();
            }

            usuario.ResetarTentativasLogin();
            await _usuarios.AtualizarAsync(usuario, cancellationToken);

            var accessToken = _token.GenerateToken(usuario);
            var refreshToken = _token.GenerateRefreshToken();

            await _refresh.SaveAsync(
                usuario.Id,
                refreshToken,
                DateTime.UtcNow.AddDays(7),
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return new DTOAuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private static AuthenticationException CredenciaisInvalidas() =>
            new("AUTH_INVALID_CREDENTIALS", "Credencial inválida!");

    }
}
