using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Domain.ValueObjects;
using Barbearia.Core.DTO;
using Barbearia.Core.Exceptions;
using Barbearia.Core.Interface;
using BarbeariaCore.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Barbearia.Core.Service
{
    public class NovoClienteService : INovoClienteService
    {

        private readonly INovoClienteRepository _repository;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<NovoClienteService> _logger;
        private readonly IPasswordHash _hash;

        public NovoClienteService(INovoClienteRepository repository, IUnitOfWork uow, ILogger<NovoClienteService> logger, IPasswordHash hash)
        {
            _repository = repository;
            _uow = uow;
            _logger = logger;
            _hash = hash;
        }

        public async Task<DTOResposta> CadastrarAsync(string nome, string email, string telefone, string cpf, string login, string senha, string foto)
        {
            nome = nome.Trim();
            login = login.Trim().ToLowerInvariant();

            // DADOS DEVEM SER VERIFICADOS NO VALUE OBJECTS
            var emailNormalizado = new Email(email);
            var telefoneNormalizado = new Phone(telefone);
            var cpfNormalizado = new Cpf(cpf);
            var senhaProtegida = Senha.Criar(senha, _hash);

            await ValidarDuplicidadeAsync(emailNormalizado.EmailPessoa, cpfNormalizado.Numero, telefoneNormalizado.Telefone, login);

            var novo_usuario = new Usuario
            (
                nome,
                emailNormalizado,
                telefoneNormalizado,
                cpfNormalizado,
                login,
                senhaProtegida,
                Enum.RolePerson.Cliente,
                true,
                foto
            );

            await _repository.CadastraNovoClienteAsync(novo_usuario);

            await _uow.SaveChangesAsync();
            _logger.LogInformation("Usuário novo cadastrado com o login: {login}", login);
            return new DTOResposta
            {
                Sucesso = true,
                Mensagem = "Usuário cadastrado com sucesso!"

            };
        }

        private async Task ValidarDuplicidadeAsync(string email, string cpf, string telefone, string login)
        {
            var existente = await _repository.VerificarDuplicidadeAsync(
            email,
            cpf,
            telefone,
            login);

            if (existente is null)
                return;

            if (existente.Email.EmailPessoa == email)
            {
                _logger.LogWarning("Tentativa de cadastro com e-mail já existente");
                throw new DomainException("USER_EMAIL_ALREADY_EXISTS", "E-mail já cadastrado!");
            }

            if (existente.CPF.Numero == cpf)
            {
                _logger.LogWarning("Tentativa de cadastro com CPF já existente");
                throw new DomainException("USER_CPF_ALREADY_EXISTS", "CPF já cadastrado!");
            }

            if (existente.Phone.Telefone == telefone)
            {
                _logger.LogWarning("Tentativa de cadastro com telefone já existente");
                throw new DomainException("USER_PHONE_ALREADY_EXISTS", "Telefone já cadastrado!");
            }

            if (existente.Login == login)
            {
                _logger.LogWarning(
                    "Tentativa de cadastro com login já existente: {Login}",
                    login);
                throw new DomainException("USER_LOGIN_ALREADY_EXISTS", "Login já cadastrado!");
            }
        }
    }
}
