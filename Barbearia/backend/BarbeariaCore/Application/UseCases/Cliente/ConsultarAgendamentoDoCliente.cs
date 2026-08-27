using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Exceptions;

namespace BarbeariaCore.UseCases.Cliente
{
    public sealed class ConsultarAgendamentoDoCliente
    {

        private readonly IAgendamentoRepository _agendamentos;

        public ConsultarAgendamentoDoCliente(IAgendamentoRepository agendamentos)
        {
            _agendamentos = agendamentos;
        }

        public async Task<DTOHorarioDetalhes?> ExecutarAsync(
            int agendamentoId,
            int clienteId)
        {
            var agendamento =
                await _agendamentos.ObterPorIdAsync(
                    agendamentoId);

            if (agendamento is null)
                return null;

            if (agendamento.ClienteId != clienteId)
            {
                throw new ForbiddenException(
                    "RESOURCE_ACCESS_DENIED",
                    "Você não possui acesso a este agendamento.");
            }

            return ConsultarAgendamento.Mapear(
                agendamento);
        }
    }
}
