using BarbeariaCore.Domain.Enum;

namespace BarbeariaCore.Application.DTOs;

// CRIADO PARA RETORNAR OS HORÁRIOS SEM QUE TENHAM ACESSO AO DOMINIO QUANDO FOSSE RETORNAR AS INFORMAÇÕES
public sealed class DTOHorarioDetalhes
{
    public int Id { get; init; }
    public int ClienteId { get; init; }
    public int BarbeiroId { get; init; }
    public int ServicoId { get; init; }
    public DateTime Horario { get; init; }
    public StatusAgendamento Status { get; init; }
}
