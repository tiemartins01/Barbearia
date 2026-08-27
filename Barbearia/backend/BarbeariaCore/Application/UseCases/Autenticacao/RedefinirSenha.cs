using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Policies;
using BarbeariaCore.Domain.ValueObjects;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;
using ValidationException = BarbeariaCore.Exceptions.ValidationException;

namespace BarbeariaCore.UseCases.Autenticacao
{
    public sealed class RedefinirSenha
    {

        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHash _passwordHash;

        public RedefinirSenha(
            IUsuarioRepository usuarios,
            IUnitOfWork uow,
            IPasswordHash passwordHash)
        {
            _usuarios = usuarios;
            _uow = uow;
            _passwordHash = passwordHash;
        }

        public async Task<DTOResposta> ExecutarAsync(
            string codigo,
            string email,
            string senha,
            string senhaRepetida)
        {
            email = email.Trim().ToLowerInvariant();
            var agora = DateTime.Now;

            var usuario = await _usuarios.ObterPorEmailAsync(email);

            if (usuario is null || !usuario.Ativado)
                throw new AuthenticationException(
                    "PASSWORD_RESET_INVALID_DATA",
                    "Dados inválidos!");

            PoliticaSenha.Validar(senha);

            if (!string.Equals(senha, senhaRepetida, StringComparison.Ordinal))
                throw new ValidationException(
                    "PASSWORD_RESET_PASSWORD_MISMATCH",
                    "Dados inválidos!");

            if (!usuario.CodigoIsValido(agora))
                throw new AuthenticationException(
                    "PASSWORD_RESET_CODE_EXPIRED",
                    "Código expirado! Solicite um novo código!");

            if (!usuario.PodeTrocarSenha(codigo, agora))
            {
                usuario.RegistrarFalhaTrocaSenha();
                await _usuarios.AtualizarAsync(usuario);
                await _uow.SaveChangesAsync();

                throw new AuthenticationException(
                    "PASSWORD_RESET_INVALID_CODE",
                    "Código de recuperação inválido.");
            }

            var senhaHash = _passwordHash.Hash(senha);
            var senhaDominio = Senha.DeHash(senhaHash);

            usuario.AlterarSenha(senhaDominio);

            await _usuarios.AtualizarAsync(usuario);
            await _uow.SaveChangesAsync();

            return new DTOResposta
            {
                Sucesso = true,
                Mensagem = "Senha alterada!"
            };
        }

    }
}
