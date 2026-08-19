using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces
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
