using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;

namespace BarbeariaCore.UseCases.Servicos
{
    public sealed class ListarServicosAtivos
    {
        private readonly IServicosAtivosQuery _query;

        public ListarServicosAtivos(IServicosAtivosQuery query)
        {
          _query = query;
        }
        public Task<IReadOnlyList<DTOServicosAtivos>> ExecutarAsync(CancellationToken cancellationToken)
        {
            return _query.ListarAsync(cancellationToken);
        }

    }
}
