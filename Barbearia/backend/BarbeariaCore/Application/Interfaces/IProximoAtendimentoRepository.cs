using Barbearia.Core.Domain.Entities;
using Barbearia.Core.DTO;

namespace Barbearia.Core.Interface
{
    public interface IProximoAtendimentoRepository
    {
        Task<DTOProximoAgendamento?> InfoProximoAgendamento(int id);
        Task MarcarAgendamento(Horarios horario);
        Task<bool> DisponibilidadeHorario(DateTime horario, int id_barbeiro);
        Task<bool> BarbeiroExiste(int id_barbeiro);
        Task<bool> ServicoExiste(int id_servico);
        Task<List<TimeOnly>> BuscarHorariosOcupadosAsync(int idBarbeiro,DateOnly data);
    }
}
