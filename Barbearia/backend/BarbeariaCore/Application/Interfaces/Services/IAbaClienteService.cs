using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Services
{
    public interface IAbaClienteService
    {
        Task<List<DTOBarbeiro>> BuscarBarbeiros();
        Task<List<DTOHistorico>> HistoricoCliente(int idCliente, int page, int pageSize);
        Task<DTODadosPessoais?> DadosPessoaisAsync(int idCliente);
        Task AlterandoDados(DTOAlterandoDados dados);
        Task RealizandoAvaliacaoAsync(DTOAvaliacao avaliacao, int idCliente);
        Task<DTOHorarioDetalhes?> InfoHorario(int id);
        Task<DTOHorarioDetalhes?> InfoHorarioDoCliente(int id, int userId);
    }
}
