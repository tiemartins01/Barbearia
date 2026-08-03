using Barbearia.Core.DTO;
using Barbearia.Core.Interface;

namespace Barbearia.Core.Service
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
