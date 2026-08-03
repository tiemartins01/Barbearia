using Barbearia.Core.DTO;

namespace Barbearia.Core.Interface
{
    public interface IServicosRepository 
    {

        Task <List<DTOServicosAtivos>> GetServicosAtivos();

    }
}
