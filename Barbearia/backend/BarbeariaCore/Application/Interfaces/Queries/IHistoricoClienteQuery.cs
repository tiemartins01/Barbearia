using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Queries
{
    public interface IHistoricoClienteQuery
    {
        Task<IReadOnlyList<DTOHistorico>> ConsultarAsync(
            int clienteId,
            int page,
            int pageSize);
    }
}
