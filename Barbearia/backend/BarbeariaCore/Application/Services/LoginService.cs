using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Application.Interfaces.Services;
using BarbeariaCore.Domain.Entities;
using Microsoft.Extensions.Logging;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;

namespace BarbeariaCore.Application.Services
{
    public sealed class LoginService : ILoginService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly ITokenService _token;
        private readonly IRefreshRepository _refresh;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<LoginService> _logger;
        private readonly IPasswordHash _passwordHash;

        public LoginService(
            IUsuarioRepository usuarios,
            ITokenService token,
            IRefreshRepository refresh,
            IUnitOfWork uow,
            ILogger<LoginService> logger,
            IPasswordHash passwordHash)
        {
            _usuarios = usuarios;
            _token = token;
            _refresh = refresh;
            _uow = uow;
            _logger = logger;
            _passwordHash = passwordHash;
        }

        public async Task<DTOAuthResponse> RealizarLoginAsync(string login, string senha)
        {
            login = login.Trim().ToLowerInvariant();
            var agora = DateTime.Now;

            var usuario = await _usuarios.ObterPorLoginAsync(login);

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
                await _usuarios.AtualizarAsync(usuario);
                await _uow.SaveChangesAsync();

                _logger.LogWarning("Senha incorreta para {Login}", login);
                throw CredenciaisInvalidas();
            }

            usuario.ResetarTentativasLogin();
            await _usuarios.AtualizarAsync(usuario);

            var accessToken = _token.GenerateToken(usuario);
            var refreshToken = _token.GenerateRefreshToken();

            await _refresh.SaveAsync(
                usuario.Id,
                refreshToken,
                DateTime.UtcNow.AddDays(7));

            await _uow.SaveChangesAsync();

            return new DTOAuthResponse
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            };
        }

        private static AuthenticationException CredenciaisInvalidas() =>
            new("AUTH_INVALID_CREDENTIALS", "Credencial inválida!");
    }
}
