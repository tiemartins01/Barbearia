using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Application.Interfaces.Services;
using BarbeariaCore.Security;
using Microsoft.Extensions.Logging;

namespace BarbeariaCore.Application.Services
{
    public sealed class EmailEsqueciSenhaService : IEmailEsqueciSenhaService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IEnviarEmail _enviar;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<EmailEsqueciSenhaService> _logger;

        public EmailEsqueciSenhaService(
            IUsuarioRepository usuarios,
            IEnviarEmail enviar,
            IUnitOfWork uow,
            ILogger<EmailEsqueciSenhaService> logger)
        {
            _usuarios = usuarios;
            _enviar = enviar;
            _uow = uow;
            _logger = logger;
        }

        public async Task EnviarEmailAsync(string email)
        {
            email = email.Trim().ToLowerInvariant();

            var usuario = await _usuarios.ObterPorEmailAsync(email);

            if (usuario is null)
            {
                _logger.LogWarning("Tentativa de recuperação para e-mail inexistente.");
                return;
            }

            var agora = DateTime.Now;
            var codigo = CodeGenerator.GerarCod();

            usuario.GerarCodigo(codigo, agora);

            await _usuarios.AtualizarAsync(usuario);
            await _uow.SaveChangesAsync();

            var mensagem = $"""
                <p>Olá {usuario.Nome},</p>
                <p>Percebemos que você solicitou o código para troca de senha.</p>
                <p>Código: <span style="font-size:16px;">{codigo}</span></p>
                <p style="font-size:8px">Caso não tenha sido você, descarte este e-mail.</p>
                """;

            await _enviar.EnviarEmailAsync(
                usuario.Email.Valor,
                "Troca de senha",
                mensagem);

            _logger.LogInformation(
                "Código de recuperação enviado para {Email}",
                email);
        }
    }
}
