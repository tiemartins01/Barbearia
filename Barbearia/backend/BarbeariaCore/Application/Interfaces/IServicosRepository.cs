using BarbeariaCore.Application.DTOs;

namespace BarbeariaCore.Application.Interfaces
{
    public interface IServicosRepository 
    {

        Task <List<DTOServicosAtivos>> GetServicosAtivos();

    }
}
