using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;

namespace BarbeariaCore.UseCases.Cliente
{
    public sealed class ConsultarDadosPessoais
    {
        private readonly IDadosPessoaisQuery _query;

        public ConsultarDadosPessoais(IDadosPessoaisQuery query)
        {
            _query = query;
        }

        public Task<DTODadosPessoais?> ExecutarAsync(int idCliente)
        {
            return _query.ConsultarAsync(idCliente);
        }
    }
}
