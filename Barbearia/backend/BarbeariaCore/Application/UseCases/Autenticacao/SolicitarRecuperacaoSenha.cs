using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Security;
using Microsoft.Extensions.Logging;

namespace BarbeariaCore.UseCases.Autenticacao
{
    public sealed class SolicitarRecuperacaoSenha
    {

        private readonly IUsuarioRepository _usuarios;
        private readonly IEnviarEmail _enviar;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<SolicitarRecuperacaoSenha> _logger;
        private readonly ICodigoRecuperacaoGenerator _codigoGenerator;

        public SolicitarRecuperacaoSenha(
            IUsuarioRepository usuarios,
            IEnviarEmail enviar,
            IUnitOfWork uow,
            ILogger<SolicitarRecuperacaoSenha> logger,
            ICodigoRecuperacaoGenerator codigoGenerator)
        {
            _usuarios = usuarios;
            _enviar = enviar;
            _uow = uow;
            _logger = logger;
            _codigoGenerator = codigoGenerator;
        }

        public async Task ExecutarAsync(string email, CancellationToken cancellationToken = default)
        {
            email = email.Trim().ToLowerInvariant();

            var usuario = await _usuarios.ObterPorEmailAsync(email, cancellationToken);

            if (usuario is null)
            {
                _logger.LogWarning("Tentativa de recuperação para e-mail inexistente.");
                return;
            }

            var agora = DateTime.UtcNow;
            var codigo = _codigoGenerator.Gerar();

            usuario.GerarCodigo(codigo, agora);

            await _usuarios.AtualizarAsync(usuario, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var mensagem = $"""
                <p>Olá {usuario.Nome},</p>
                <p>Percebemos que você solicitou o código para troca de senha.</p>
                <p>Código: <span style="font-size:16px;">{codigo}</span></p>
                <p style="font-size:8px">Caso não tenha sido você, descarte este e-mail.</p>
                """;

            await _enviar.EnviarEmailAsync(
                usuario.Email.Valor,
                "Troca de senha",
                mensagem, cancellationToken);

            _logger.LogInformation(
                "Código de recuperação enviado para {Email}",
                email);
        }

    }
}
