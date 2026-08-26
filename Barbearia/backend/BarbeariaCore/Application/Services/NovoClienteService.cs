using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Application.Interfaces.Services;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.Exceptions;
using BarbeariaCore.Domain.Policies;
using BarbeariaCore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BarbeariaCore.Application.Services
{
    public sealed class NovoClienteService : INovoClienteService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<NovoClienteService> _logger;
        private readonly IPasswordHash _hash;

        public NovoClienteService(
            IUsuarioRepository usuarios,
            IUnitOfWork uow,
            ILogger<NovoClienteService> logger,
            IPasswordHash hash)
        {
            _usuarios = usuarios;
            _uow = uow;
            _logger = logger;
            _hash = hash;
        }

        public async Task<DTOResposta> CadastrarAsync(
            string nome,
            string email,
            string telefone,
            string cpf,
            string login,
            string senha,
            string foto)
        {
            nome = nome.Trim();
            login = login.Trim().ToLowerInvariant();

            var emailNormalizado = new Email(email);
            var telefoneNormalizado = new Telefone(telefone);
            var cpfNormalizado = new Cpf(cpf);

            PoliticaSenha.Validar(senha);

            await ValidarUnicidadeAsync(
                emailNormalizado,
                cpfNormalizado,
                telefoneNormalizado,
                login);

            var senhaHash = _hash.Hash(senha);
            var senhaDominio = Senha.DeHash(senhaHash);

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

            await _usuarios.AdicionarAsync(novoUsuario);

            await _uow.SaveChangesAsync();

            novoUsuario.RegistrarCriacao();

            await _uow.SaveChangesAsync();

            _logger.LogInformation(
                "Usuário novo cadastrado com o login: {Login}",
                login);

            return new DTOResposta
            {
                Sucesso = true,
                Mensagem = "Usuário cadastrado com sucesso!"
            };
        }

        private async Task ValidarUnicidadeAsync(
            Email email,
            Cpf cpf,
            Telefone telefone,
            string login)
        {
            if (await _usuarios.ObterPorEmailAsync(email.Valor) is not null)
                throw new DomainException("USER_EMAIL_ALREADY_EXISTS", "E-mail já cadastrado.");

            if (await _usuarios.ObterPorCpfAsync(cpf.Valor) is not null)
                throw new DomainException("USER_CPF_ALREADY_EXISTS", "CPF já cadastrado.");

            if (await _usuarios.ObterPorTelefoneAsync(telefone.Valor) is not null)
                throw new DomainException("USER_PHONE_ALREADY_EXISTS", "Telefone já cadastrado.");

            if (await _usuarios.ObterPorLoginAsync(login) is not null)
                throw new DomainException("USER_LOGIN_ALREADY_EXISTS", "Login já cadastrado.");
        }
    }
}
