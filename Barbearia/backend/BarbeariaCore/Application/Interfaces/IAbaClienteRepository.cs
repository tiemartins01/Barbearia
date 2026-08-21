using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces
{
    public interface IAbaClienteRepository
    {
        Task<List<DTOBarbeiro>> BuscarTodosBarbeiros();
        Task<List<DTOHistorico>> Historico(int id, int page, int pageSize);
        Task<DTODadosPessoais> DadosPessoais(int id);
        Task<Usuario?> GetUsuarioAsync(int id);
        Task<Agendamento?> HorarioValidoAsync(int id);
        Task RealizarAvaliacaoAsync(Avaliacao avaliacao);
        Task<Agendamento?> BuscarHorarioParaAtualizarAsync(int id);

    }
}
