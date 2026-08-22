using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using Microsoft.Extensions.Logging;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;

namespace BarbeariaCore.Application.Services
{
    public class LoginService : ILoginService
    {

        private readonly ILoginRepository _repository;
        private readonly ITokenService _token;
        private readonly IRefreshRepository _refresh;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<LoginService> _logger;
        private readonly IPasswordHash _passwordHash;

        public LoginService (ILoginRepository repository, ITokenService token, IRefreshRepository refresh, IUnitOfWork uow, ILogger<LoginService> logger, IPasswordHash password)
        {
            _repository = repository;
            _token = token;
            _refresh = refresh;
            _uow = uow;
            _logger = logger;
            _passwordHash = password;
        }

        public async Task<DTOAuthResponse> RealizarLoginAsync(string login, string senha)
        {
            // DTO VALIDATOR LOGIN JÁ VERIFICA E NÃO DEIXA CHEGAR VAZIO
            login = login.Trim().ToLowerInvariant();

            var usuario = await _repository.ObterPorLoginAsync(login);

            if (usuario == null)
            {
                _logger.LogWarning("Tentativa de acessar com um usuário inexistente: {login}", login);
                throw new AuthenticationException("AUTH_INVALID_CREDENTIALS", "Credencial inválida!");
            }
               
            ValidarUsuario(usuario, login);

            if (!_passwordHash.Verify(senha, usuario.Senha.Hash)) 
            {
                await RegistrarFalha(usuario, login);
                throw new AuthenticationException("AUTH_INVALID_CREDENTIALS", "Credencial inválida!");
            }

            usuario.ResetarTentativasLogin();

            await _repository.Atualizar(usuario);

            var response = await GerarTokensAsync(usuario);

            _logger.LogInformation("Login realizado e tokens criados {id}", usuario.Id);

            return response;
        }

        private void ValidarUsuario(Usuario usuario, string login)
        {
            var agora = DateTime.Now;

            if (!usuario.Ativado)
            {
                _logger.LogWarning("Tentativa de logar com um usuário inátivo {login}",login);
                throw new AuthenticationException("AUTH_INVALID_CREDENTIALS", "Credencial inválida!");
            }

            if (!usuario.PodeLogar(agora))
            {
                _logger.LogWarning("Tentativa de logar com um usuário bloqueado {login}", login);
                throw new AuthenticationException("AUTH_INVALID_CREDENTIALS", "Credencial inválida!");
            }
        }

        private async Task RegistrarFalha(Usuario usuario, string login)
        {
            var agora = DateTime.Now;

            usuario.RegistrarFalhaLogin(agora);
            await _repository.Atualizar(usuario);
            await _uow.SaveChangesAsync();
            _logger.LogWarning("Tentativa de logar com uma senha incorreta {login}", login);
        }

        private async Task<DTOAuthResponse> GerarTokensAsync(Usuario usuario)
        {
            var access_token = _token.GenerateToken(usuario);
            var refresh = _token.GenerateRefreshToken();

            await _refresh.SaveAsync(usuario.Id, refresh, DateTime.UtcNow.AddDays(7));

            await _uow.SaveChangesAsync();


            return new DTOAuthResponse
            {
                accessToken = access_token,
                refreshToken = refresh
            };
        }
    }
}
