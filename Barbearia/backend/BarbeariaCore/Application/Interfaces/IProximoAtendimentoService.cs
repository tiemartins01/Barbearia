using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces
{
    public interface IProximoAtendimentoService
    {

        Task<DTOProximoAgendamento> ObterProximoAtendimentoAsync(int id);
        Task<DTOResposta> AgendarHorarioAsync(int id_barbeiro, int id_usuario, int id_servico, DateTime horario);
        Task<IReadOnlyCollection<TimeOnly>> ObterHorariosDisponiveisAsync(int idBarbeiro, DateOnly data);
    }
}
