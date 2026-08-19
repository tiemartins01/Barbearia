using BarbeariaCore.Application.DTOs;
namespace BarbeariaCore.Application.Interfaces
{
    public interface IServicosService
    {
        Task<List<DTOServicosAtivos>> CarregarServicosAtivos();
    }
}
