using Barbearia.Core.Enum;

namespace Barbearia.Core.DTO;

// CRIADO PARA RETORNAR OS HORÁRIOS SEM QUE TENHAM ACESSO AO DOMINIO QUANDO FOSSE RETORNAR AS INFORMAÇÕES
public sealed class DTOHorarioDetalhes
{
    public int Id { get; init; }
    public int IdCliente { get; init; }
    public int IdBarbeiro { get; init; }
    public int IdServico { get; init; }
    public DateTime Horario { get; init; }
    public StatusAgendamento Status { get; init; }
}
