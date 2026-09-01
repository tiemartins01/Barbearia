using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Exceptions;
using Microsoft.Extensions.Logging;

namespace BarbeariaCore.UseCases.Cliente
{
    public sealed class AvaliarAtendimento
    {

        private readonly IAgendamentoRepository _agendamento;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AvaliarAtendimento> _logger;
        private readonly IAvaliacaoRepository _avaliacoes;


        public AvaliarAtendimento(IAgendamentoRepository agendamento, 
            IUnitOfWork uow, ILogger<AvaliarAtendimento> logger,
            IAvaliacaoRepository avaliacoes)
        {
            _agendamento = agendamento;
            _uow = uow;
            _logger = logger;
            _avaliacoes = avaliacoes;
        }

        public async Task ExecutarAsync(
            DTOAvaliacao avaliacao,
            int idCliente, CancellationToken cancellationToken)
        {
            var agendamento =
                await _agendamento.ObterPorIdAsync(
                    avaliacao.AgendamentoId, cancellationToken);

            if (agendamento is null)
                throw new NotFoundException(
                    "APPOINTMENT_NOT_FOUND",
                    "Agendamento não encontrado.");

            if (agendamento.ClienteId != idCliente)
                throw new ForbiddenException(
                    "RESOURCE_ACCESS_DENIED",
                    "Você não possui acesso a este agendamento.");

            if (await _avaliacoes.ExisteParaAgendamentoAsync(agendamento.Id, cancellationToken))
                throw new ConflictException(
                    "REVIEW_ALREADY_EXISTS",
                    "Este atendimento já possui avaliação.");

            var agora = DateTime.UtcNow;

            // Autoridade do status fica no Aggregate.
            agendamento.MarcarComoAvaliado(agora);

            var novaAvaliacao = new Avaliacao(
                agendamento.BarbeiroId,
                agendamento.ClienteId,
                agendamento.Id,
                avaliacao.Nota,
                avaliacao.Comentario,
                agendamento.DataAgendamento,
                agendamento.ServicoId);

            await _avaliacoes.AdicionarAsync(novaAvaliacao, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Avaliação realizada. Cliente={ClienteId} Agendamento={AgendamentoId}",
                idCliente,
                agendamento.Id);
        }
    }
}
