using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Domain.Policies;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;
using ValidationException = BarbeariaCore.Exceptions.ValidationException;
namespace BarbeariaCore.Application.Services
{
    public sealed class TrocaSenhaService : ITrocaSenhaService
    {

        private readonly ITrocaSenhaRepository _repository;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHash _passwordHash;
        public TrocaSenhaService(ITrocaSenhaRepository repository, IUnitOfWork uow, IPasswordHash passwordhash)
        {
            _repository = repository;
            _uow = uow;
            _passwordHash = passwordhash;

        }

        public async Task<DTOResposta> RealizarTrocaSenha(string codigo, string email, string senha, string senharepetida)
        {
            var usuario =
                await _repository.PegaInformacaoUsuario(email);

            if (usuario is null || !usuario.Ativado)
            {
                throw new AuthenticationException(
                    "PASSWORD_RESET_INVALID_DATA",
                    "Dados inválidos!");
            }

            PoliticaSenha.Validar(senha);

            if (!string.Equals(
                    senha,
                    senharepetida,
                    StringComparison.Ordinal))
            {
                throw new ValidationException(
                    "PASSWORD_RESET_PASSWORD_MISMATCH",
                    "Dados inválidos!");
            }

            if (!usuario.CodigoIsValido())
            {
                throw new AuthenticationException(
                    "PASSWORD_RESET_CODE_EXPIRED",
                    "Código expirado! Solicite um novo código!");
            }

            if (!usuario.PodeTrocarSenha(codigo))
            {
                await RegistrarFalhaAsync(usuario);

                throw new AuthenticationException(
                    "PASSWORD_RESET_INVALID_CODE",
                    "Código de recuperação inválido.");
            }

            var senhaHash =
                _passwordHash.Hash(senha);

            var senhaDominio =
                Senha.DeHash(senhaHash);

            usuario.AlterarSenha(senhaDominio);

            await _repository.AtualizaUsuario(usuario);

            await _uow.SaveChangesAsync();

            return new DTOResposta
            {
                Sucesso = true,
                Mensagem = "Senha alterada!"
            };
        }

        private async Task RegistrarFalhaAsync(Usuario usuario)
        {
            usuario.RegistrarFalhaTrocaSenha();
            await _repository.AtualizaUsuario(usuario);
            await _uow.SaveChangesAsync();
        }

    }
}
