using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;

namespace BarbeariaCore.UseCases.Cliente
{
    public sealed class ListarBarbeiros
    {

        private readonly IBarbeirosQuery _query;

        public ListarBarbeiros(IBarbeirosQuery query)
        {
            _query = query;
        }

        public async Task<IReadOnlyList<DTOBarbeiro>> ExecutarAsync()
        {
            return await _query.ListarAtivosAsync();
        }


    }
}
