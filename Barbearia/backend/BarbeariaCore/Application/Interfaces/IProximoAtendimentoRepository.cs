using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Models;
using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces
{
    public interface IProximoAtendimentoRepository
    {
        Task<DTOProximoAgendamento?> InfoProximoAgendamento(
            int idUsuario);

        Task MarcarAgendamento(
            Agendamento agendamento);

        Task<bool> ExisteConflitoAsync(
            int barbeiroId,
            DateTime inicio,
            DateTime fim);

        Task<bool> BarbeiroExiste(
            int barbeiroId);

        Task<Servico?> ObterServicoAsync(
            int servicoId);

        Task<IReadOnlyList<PeriodoOcupado>>
            BuscarPeriodosOcupadosAsync(
                int barbeiroId,
                DateOnly data);
    }
}