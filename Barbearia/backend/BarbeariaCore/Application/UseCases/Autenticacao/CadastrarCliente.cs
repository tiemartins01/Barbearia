using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Application.Policies;
using BarbeariaCore.Domain.Policies;
using BarbeariaCore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BarbeariaCore.UseCases.Autenticacao
{
    public sealed class CadastrarCliente
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CadastrarCliente> _logger;
        private readonly IPasswordHash _hash;
        private readonly UsuarioUnicidadePolicy _policy;

        public CadastrarCliente(IUsuarioRepository usuarios, IUnitOfWork uow,
            ILogger<CadastrarCliente> logger, IPasswordHash hash,
            UsuarioUnicidadePolicy policy) 
        { 
            _uow = uow;
            _logger = logger;   
            _hash = hash;
            _usuarios = usuarios;
            _policy = policy;
        }

        public async Task<DTOResposta> ExecutarAsync(
           string nome,
           string email,
           string telefone,
           string cpf,
           string login,
           string senha,
           string foto,
           CancellationToken cancellationToken)
        {
            nome = nome.Trim();
            login = login.Trim().ToLowerInvariant();

            var emailNormalizado = new Email(email);
            var telefoneNormalizado = new Telefone(telefone);
            var cpfNormalizado = new Cpf(cpf);

            PoliticaSenha.Validar(senha);

            await _policy.ValidarAsync(
                emailNormalizado,
                cpfNormalizado,
                telefoneNormalizado,
                login,
                cancellationToken);

            var senhaHash = _hash.Hash(senha);
            var senhaDominio = Senha.DeHash(senhaHash);

            var agora = DateTime.UtcNow;

            var novoUsuario = new Usuario(
                nome,
                emailNormalizado,
                telefoneNormalizado,
                cpfNormalizado,
                login,
                senhaDominio,
                RolePerson.Cliente,
                true,
                foto);

            await _usuarios.AdicionarAsync(novoUsuario, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            novoUsuario.RegistrarCriacao(agora);

            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Usuário novo cadastrado com o login: {Login}",
                login);

            return new DTOResposta
            {
                Sucesso = true,
                Mensagem = "Usuário cadastrado com sucesso!"
            };
        }
    }
}
