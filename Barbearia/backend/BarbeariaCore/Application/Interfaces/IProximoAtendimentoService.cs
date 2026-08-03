using Barbearia.Core.DTO;

namespace Barbearia.Core.Interface
{
    public interface IProximoAtendimentoService
    {

        Task<DTOProximoAgendamento> ObterProximoAtendimentoAsync(int id);
        Task<DTOResposta> AgendarHorarioAsync(int id_barbeiro, int id_usuario, int id_servico, DateTime horario);
        Task<IReadOnlyCollection<TimeOnly>> ObterHorariosDisponiveisAsync(int idBarbeiro, DateOnly data);
    }
}
