using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Queries
{
    public interface IServicosAtivosQuery
    {
        Task<IReadOnlyList<DTOServicosAtivos>> ListarAsync(CancellationToken cancellationToken = default);
    }
}
