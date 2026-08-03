using Barbearia.Core.DTO;
namespace Barbearia.Core.Interface
{
    public interface IServicosService
    {
        Task<List<DTOServicosAtivos>> CarregarServicosAtivos();
    }
}
