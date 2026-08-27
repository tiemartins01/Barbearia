using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbeariaCore.UseCases.Cliente
{
    public sealed class ConsultarAgendamento
    {
        private readonly IAgendamentoRepository _agendamentos;

        public ConsultarAgendamento(IAgendamentoRepository agendamentos)
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

            return agendamento is null
                ? null
                : Mapear(agendamento);
        }

        internal static DTOHorarioDetalhes Mapear(
            Agendamento agendamento)
        {
            return new DTOHorarioDetalhes
            {
                Id = agendamento.Id,
                IdCliente = agendamento.ClienteId,
                IdBarbeiro = agendamento.BarbeiroId,
                IdServico = agendamento.ServicoId,
                Horario = agendamento.DataAgendamento,
                Status = agendamento.Status
            };
        }

    }
}
