using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Domain.Enum;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Queries
{
    public sealed class ProximoAgendamentoQuery : IProximoAgendamentoQuery
    {
        private readonly AppDbContext _context;

        public ProximoAgendamentoQuery(AppDbContext context)
        {
            _context = context;
        }

        public Task<DTOProximoAgendamento?> ObterAsync(
            int clienteId,
            DateTime agora)
        {
            return _context.Agendamentos
                .AsNoTracking()
                .Where(x =>
                    x.ClienteId == clienteId &&
                    x.DataAgendamento > agora &&
                    x.Status == StatusAgendamento.Agendado)
                .OrderBy(x => x.DataAgendamento)
                .Select(x => new DTOProximoAgendamento
                {
                    Horario = x.DataAgendamento.ToString("yyyy-MM-ddTHH:mm:ss"),
                    NomeBarbeiro = x.Barbeiro.Usuario.Nome,
                    NomeServico = x.Servico.Nome
                })
                .FirstOrDefaultAsync();
        }
    }
}
