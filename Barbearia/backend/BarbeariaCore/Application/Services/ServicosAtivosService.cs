using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Application.Interfaces.Services;

namespace BarbeariaCore.Application.Services
{
    public sealed class ServicosAtivosService : IServicosService
    {
        private readonly IServicosAtivosQuery _query;

        public ServicosAtivosService(IServicosAtivosQuery query)
        {
            _query = query;
        }

        public async Task<List<DTOServicosAtivos>> CarregarServicosAtivos()
        {
            var servicos = await _query.ListarAsync();
            return servicos.ToList();
        }
    }
}
