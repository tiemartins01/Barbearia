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

        public Task<Avaliacao?> ObterPorIdAsync(int avaliacaoId) =>
            _context.Avaliacoes.FirstOrDefaultAsync(x => x.Id == avaliacaoId);

        public Task<bool> ExisteParaAgendamentoAsync(int agendamentoId) =>
            _context.Avaliacoes
                .AsNoTracking()
                .AnyAsync(x => x.AgendamentoId == agendamentoId);

        public async Task AdicionarAsync(Avaliacao avaliacao) =>
            await _context.Avaliacoes.AddAsync(avaliacao);
    }
}
