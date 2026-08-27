using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;
using ValidationException = BarbeariaCore.Exceptions.ValidationException;


namespace BarbeariaCore.UseCases.Agendamentos
{
    public sealed class ConsultarProximoAgendamento
    {

        private readonly IProximoAgendamentoQuery _proximoAgendamentoQuery;

        public ConsultarProximoAgendamento (IProximoAgendamentoQuery proximoAgendamentoQuery) 
        { 
            _proximoAgendamentoQuery = proximoAgendamentoQuery;
        }

        public async Task<DTOProximoAgendamento?> ExecutarAsync(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ValidationException(
                    "USER_ID_INVALID",
                    "O identificador do usuário é inválido.");

            return await _proximoAgendamentoQuery.ObterAsync(
                idUsuario,
                DateTime.Now);
        }

    }
}
