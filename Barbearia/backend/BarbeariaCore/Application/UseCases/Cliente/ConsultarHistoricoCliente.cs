using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.UseCases.Cliente
{
    public sealed class ConsultarHistoricoCliente
    {

        private readonly IHistoricoClienteQuery _query;

        public ConsultarHistoricoCliente(IHistoricoClienteQuery query)
        {
            _query = query;
        }

        public async Task<IReadOnlyList<DTOHistorico>> ExecutarAsync(int idCliente,
            int page,
            int pageSize)
        {
            return await _query.ConsultarAsync(idCliente, page, pageSize);
        }

    }
}
