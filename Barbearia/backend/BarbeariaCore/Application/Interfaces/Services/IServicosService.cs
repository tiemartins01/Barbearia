using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces.Services
{
    public interface IServicosService
    {
        Task<List<DTOServicosAtivos>> CarregarServicosAtivos();
    }
}
