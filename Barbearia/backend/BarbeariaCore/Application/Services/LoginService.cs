using Barbearia.Core.Domain.Entities;
using Barbearia.Core.DTO;
using Barbearia.Core.Excepetion;
using Barbearia.Core.Interface;
using Microsoft.Extensions.Logging;

namespace Barbearia.Core.Service
{
    public class LoginService : ILoginService
    {

        private readonly ILoginRepository _repository;
        private readonly ITokenService _token;
        private readonly IRefreshRepository _refresh;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<LoginService> _logger;

        public LoginService (ILoginRepository repository, ITokenService token, IRefreshRepository refresh, IUnitOfWork uow, ILogger<LoginService> logger)
        {
            _repository = repository;
            _token = token;
            _refresh = refresh;
            _uow = uow;
            _logger = logger;
        }

        public async Task<DTOAuthResponse> RealizarLoginAsync(string login, string senha)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new DomainException("EMPTY_FIELDS", "Login vazio");

            if(string.IsNullOrWhiteSpace(senha))
                throw new DomainException("EMPTY_FIELDS", "Senha vazia");

            login = login.Trim().ToLowerInvariant();

            var usuario = await _repository.ObterPorLoginAsync(login);

            if (usuario == null)
            {
                _logger.LogWarning("Tentativa de acessar com um usuário inexistente: {login}", login);
                throw new DomainException("AUTH_INVALID_CREDENTIALS", "Credenciais inválidas!");
            }
               
            ValidarUsuario(usuario, login);

            if (!usuario.Senha.Verify(senha))
            {
                await RegistrarFalha(usuario, login);
                throw new DomainException("AUTH_INVALID_CREDENTIALS", "Credenciais inválidas!");
            }

            usuario.ResetarTentativasLogin();

            await _repository.Atualizar(usuario);

            var response = await GerarTokensAsync(usuario); // já tem aqui o savechanges

            _logger.LogInformation("Login realizado e tokens criados {id}", usuario.Id);

            return response;
        }

        private void ValidarUsuario(Usuario usuario, string login)
        {
            if (!usuario.Ativado)
            {
                _logger.LogWarning("Tentativa de logar com um usuário inátivo {login}",login);
                throw new DomainException("AUTH_INVALID_CREDENTIALS", "Credenciais inválidas!");
            }

            if (!usuario.PodeLogar())
            {
                _logger.LogWarning("Tentativa de logar com um usuário bloqueado {login}", login);
                throw new DomainException("AUTH_INVALID_CREDENTIALS", "Credenciais inválidas!");
            }
        }

        private async Task RegistrarFalha(Usuario usuario, string login)
        {
            usuario.RegistrarFalhaLogin();
            await _repository.Atualizar(usuario);
            await _uow.SaveChangesAsync();
            _logger.LogWarning("Tentativa de logar com uma senha incorreta {login}", login);
        }

        private async Task<DTOAuthResponse> GerarTokensAsync(Usuario usuario)
        {
            var access_token = _token.GenerateToken(usuario);
            var refresh = _token.GenerateRefreshToken();

            await _refresh.SaveAsync(usuario.Id, refresh, DateTime.Now.AddDays(7));

            await _uow.SaveChangesAsync();


            return new DTOAuthResponse
            {
                accessToken = access_token,
                refreshToken = refresh
            };
        }
    }
}
