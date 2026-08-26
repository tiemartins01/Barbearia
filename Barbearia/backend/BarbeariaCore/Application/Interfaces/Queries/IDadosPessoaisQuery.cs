using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Queries
{
    public interface IDadosPessoaisQuery
    {
        Task<DTODadosPessoais?> ConsultarAsync(int clienteId);
    }
}
