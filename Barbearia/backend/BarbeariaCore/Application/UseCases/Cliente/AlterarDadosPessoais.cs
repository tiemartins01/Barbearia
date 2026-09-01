using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Policies;
using BarbeariaCore.Domain.ValueObjects;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;

namespace BarbeariaCore.UseCases.Cliente
{
    public sealed class AlterarDadosPessoais
    {

        private readonly IUnitOfWork _uow;
        private readonly IUsuarioRepository _usuarios;
        private readonly IPasswordHash _passwordHash;

        public AlterarDadosPessoais(IUnitOfWork uow, IUsuarioRepository usuarios, IPasswordHash passwordHash)
        {
            _uow = uow;
            _usuarios = usuarios;
            _passwordHash = passwordHash;
        }

        public async Task ExecutarAsync(DTOAlterandoDados dados, CancellationToken cancellationToken)
        {
            var usuario = await _usuarios.ObterPorIdAsync(dados.Id, cancellationToken);

            if (usuario is null)
                throw new AuthenticationException(
                    "AUTH_INVALID_CREDENTIALS",
                    "Credencial inválida!");

            usuario.AlterarDados(
                dados.Nome,
                new Email(dados.Email),
                new Telefone(dados.Telefone),
                new Cpf(dados.Cpf));

            if (!string.IsNullOrWhiteSpace(dados.NovaSenha))
            {
                PoliticaSenha.Validar(dados.NovaSenha);

                if (string.IsNullOrWhiteSpace(dados.SenhaAntiga) ||
                    !_passwordHash.Verify(
                        dados.SenhaAntiga,
                        usuario.Senha.Hash))
                {
                    throw new AuthenticationException(
                        "AUTH_INVALID_CREDENTIALS",
                        "Credencial inválida!");
                }

                var senhaHash = _passwordHash.Hash(dados.NovaSenha);
                var senhaDominio = Senha.DeHash(senhaHash);

                var agora = DateTime.UtcNow;

                usuario.AlterarSenhaPerfil(senhaDominio, agora);
            }

            await _usuarios.AtualizarAsync(usuario, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
        }

    }
}
