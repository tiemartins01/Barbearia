using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public sealed class AvaliacaoRepository : IAvaliacaoRepository
    {
        private readonly AppDbContext _context;

        public AvaliacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Avaliacao?> ObterPorIdAsync(int avaliacaoId, CancellationToken cancellationToken = default) =>
            _context.Avaliacoes.FirstOrDefaultAsync(x => x.Id == avaliacaoId, cancellationToken);

        public Task<bool> ExisteParaAgendamentoAsync(int agendamentoId, CancellationToken cancellationToken = default) =>
            _context.Avaliacoes
                .AsNoTracking()
                .AnyAsync(x => x.AgendamentoId == agendamentoId, cancellationToken);

        public async Task AdicionarAsync(Avaliacao avaliacao, CancellationToken cancellationToken = default) =>
            await _context.Avaliacoes.AddAsync(avaliacao, cancellationToken);
    }
}
