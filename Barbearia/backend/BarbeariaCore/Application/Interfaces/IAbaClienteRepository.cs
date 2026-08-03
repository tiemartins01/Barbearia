using Barbearia.Core.Domain.Entities;
using Barbearia.Core.DTO;
namespace Barbearia.Core.Interface
{
    public interface IAbaClienteRepository
    {

        Task<List<DTOBarbeiro>> BuscarTodosBarbeiros();
        Task<List<DTOHistorico>> Historico(int id, int page, int pageSize);
        Task<DTODadosPessoais> DadosPessoais(int id);
        Task<Usuario?> GetUsuarioAsync(int id);
        Task<Horarios?> HorarioValidoAsync(int id);
        Task RealizarAvaliacaoAsync(Avaliacoes avaliacao);
        Task<Horarios?> BuscarHorarioParaAtualizarAsync(int id);

    }
}
