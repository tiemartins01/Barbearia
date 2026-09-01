using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Queries
{
    public interface IProximoAgendamentoQuery
    {
        Task<DTOProximoAgendamento?> ObterAsync(
            int clienteId,
            DateTime agora, CancellationToken cancellationToken);
    }
}
