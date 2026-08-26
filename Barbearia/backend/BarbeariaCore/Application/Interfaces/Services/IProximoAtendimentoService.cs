using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Services
{
    public interface IProximoAtendimentoService
    {
        Task<DTOProximoAgendamento?> ObterProximoAtendimentoAsync(int idUsuario);

        Task<DTOResposta> AgendarHorarioAsync(
            int idBarbeiro,
            int idUsuario,
            int idServico,
            DateTime horario);

        Task<IReadOnlyCollection<TimeOnly>> ObterHorariosDisponiveisAsync(
            int idBarbeiro,
            int idServico,
            DateOnly data);
    }
}
