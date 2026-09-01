using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public sealed class ServicoRepository : IServicoRepository
    {
        private readonly AppDbContext _context;

        public ServicoRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Servico?> ObterPorIdAsync(int servicoId, CancellationToken cancellationToken = default) =>
            _context.Servicos.FirstOrDefaultAsync(x => x.Id == servicoId, cancellationToken);

        public Task<Servico?> ObterAtivoPorIdAsync(int servicoId, CancellationToken cancellationToken = default) =>
            _context.Servicos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == servicoId && x.Ativo, cancellationToken);

        public Task<bool> ExisteAtivoAsync(int servicoId, CancellationToken cancellationToken = default) =>
            _context.Servicos
                .AsNoTracking()
                .AnyAsync(x => x.Id == servicoId && x.Ativo,
                cancellationToken);

        public async Task AdicionarAsync(Servico servico, CancellationToken cancellationToken = default) =>
            await _context.Servicos.AddAsync(servico, cancellationToken);

        public Task AtualizarAsync(Servico servico, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.Servicos.Update(servico);
            return Task.CompletedTask;
        }
    }
}
