using BarbeariaCore.Domain.Exceptions;
using BarbeariaCore.Application.Interfaces;
using Microsoft.Extensions.Logging;
using BarbeariaCore.Security;


namespace BarbeariaCore.Application.Services
{
    public class EmailEsqueciSenhaService : IEmailEsqueciSenhaService
    {
        private readonly IEmailEsqueciSenhaRepository _repository;
        private readonly IEnviarEmail _enviar;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<EmailEsqueciSenhaService> _logger;
        public EmailEsqueciSenhaService(IEmailEsqueciSenhaRepository repository, IEnviarEmail enviar, IUnitOfWork uow, ILogger<EmailEsqueciSenhaService> logger)
        {
            _repository = repository;
            _enviar = enviar;
            _uow = uow;
            _logger = logger;
            _logger = logger;
        }

        public async Task EnviarEmailAsync(string email)
        {
            email = email.Trim().ToLowerInvariant();

            var usuario = await _repository.BuscarUsuarioPorEmailAsync (email);

            if (usuario == null)
                throw new DomainException("AUTH_INVALID_CREDENTIALS","Credenciais inválida!");

            var codigo =  CodeGenerator.GerarCod();
            
            usuario.GerarCodigo(codigo);

            await _repository.AtualizarAsync(usuario);

            await _uow.SaveChangesAsync();

            string mensagem = $"""
                <p>Olá {usuario.Nome},</p>

                <p>Percebemos que você solicitou o código para troca de senha.</p>

                <p>Código: <span style="font-size:16px";> {codigo.ToString()} </span></p>

                <p style="font-size:8px">Caso não tenha sido você que tenha realizado a troca, favor descartar o e-mail</p>
                """;  
            await _enviar.EnviarEmailAsync(usuario.Email.EmailPessoa, "Troca de senha",mensagem);
            _logger.LogInformation("Código enviado com sucesso para o e-mail {email}", email);
            _logger.LogWarning("Tentativa de recuperação para e-mail inexistente.");
        }

    }
}
