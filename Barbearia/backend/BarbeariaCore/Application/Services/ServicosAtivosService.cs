using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;

namespace BarbeariaCore.Application.Services
{
    public class ServicosAtivosService : IServicosService
    {

        private readonly IServicosRepository _repository;

        public ServicosAtivosService(IServicosRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DTOServicosAtivos>> CarregarServicosAtivos()
        {
            return await _repository.GetServicosAtivos();
        }
    }
}
