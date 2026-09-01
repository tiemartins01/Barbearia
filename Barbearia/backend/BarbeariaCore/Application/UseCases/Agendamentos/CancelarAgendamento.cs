using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Exceptions;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Exceptions;
using Microsoft.Extensions.Logging;

namespace BarbeariaCore.Application.UseCases.Agendamentos
{
    public sealed class CancelarAgendamento
    {

        private readonly IAgendamentoRepository _cancelar;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CancelarAgendamento> _logger;
        private readonly IUsuarioRepository _usuario;

        public CancelarAgendamento(IAgendamentoRepository cancelar,
            IUnitOfWork uow, ILogger<CancelarAgendamento> logger,
            IUsuarioRepository usuario)
        {
            _cancelar = cancelar;
            _uow = uow;
            _logger = logger;
            _usuario = usuario;
        }

        public async Task<DTOResposta> ExecutarAsync(
            int idUsuario,
            int idAgendamento,
            CancellationToken cancellationToken )
        {
            ValidarIds(idUsuario, idAgendamento);

            var agora = DateTime.UtcNow;

            var usuario = await _usuario.ObterPorIdAsync(idUsuario, cancellationToken);

            if (usuario is null)
                throw new NotFoundException(
                    "USER_NOT_FOUND",
                    "Usuário não encontrado.");

            var agendamento = await _cancelar.ObterPorIdAsync(idAgendamento, cancellationToken);

            if(agendamento is null)
                throw new NotFoundException(
                    "AGENDAMENTO_NOT_FOUND",
                    "Agendamento não encontrado.");
   
            if(agendamento.ClienteId != idUsuario)
                throw new ForbiddenException(
                "APPOINTMENT_NOT_OWNED_BY_USER",
                "O usuário não possui permissão para cancelar este agendamento.");

            agendamento.Cancelar(agora);

            await _cancelar.AtualizarAsync(agendamento, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);   

            return new DTOResposta
            {
                Sucesso = true,
                Mensagem = "Horário agendado com sucesso!"
            };
        }

        private static void ValidarIds(
            int idUsuario,
            int idAgendamento)
        {
            if (idUsuario <= 0)
                throw new ValidationException("USER_ID_INVALID",
                    "O identificador do usuário é inválido.");

            if (idAgendamento <= 0)
                throw new ValidationException("SERVICE_ID_INVALID",
                    "O identificador do agendamento é inválido.");
        }

    }
}
