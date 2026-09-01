using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Domain.Enum;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Queries
{
    public sealed class HistoricoClienteQuery : IHistoricoClienteQuery
    {
        private readonly AppDbContext _context;

        public HistoricoClienteQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<DTOHistorico>> ConsultarHistoricoAsync(
            int clienteId,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            return await _context.Agendamentos
                .AsNoTracking()
                .Where(x =>
                    x.ClienteId == clienteId &&
                    (x.Status == StatusAgendamento.Avaliado ||
                     x.Status == StatusAgendamento.Concluido))
                .OrderByDescending(x => x.DataAgendamento)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DTOHistorico
                {
                    Id = x.Id,
                    NomeServico = x.Servico.Nome,
                    NomeBarbeiro = x.Barbeiro.Usuario.Nome,
                    ValorServico = x.Servico.Preco,
                    Data = x.DataAgendamento,
                    PodeAvaliar = x.Status == StatusAgendamento.Concluido
                })
                .ToListAsync(cancellationToken);
        }
    }
}
