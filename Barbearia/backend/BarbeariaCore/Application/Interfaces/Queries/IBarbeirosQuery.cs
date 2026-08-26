using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Queries
{
    public interface IBarbeirosQuery
    {
        Task<IReadOnlyList<DTOBarbeiro>> ListarAtivosAsync();
    }
}
