using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Queries
{
    public interface IDadosPessoaisQuery
    {
        Task<DTODadosPessoais?> ConsultarDadosPessoaisAsync(int clienteId, CancellationToken cancellationToken);
    }
}
