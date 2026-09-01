using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public sealed class BarbeiroRepository : IBarbeiroRepository
    {
        private readonly AppDbContext _context;

        public BarbeiroRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Barbeiro?> ObterPorIdAsync(int barbeiroId, CancellationToken cancellationToken = default) =>
            _context.Barbeiros.FirstOrDefaultAsync(x => x.Id == barbeiroId, cancellationToken);

        public Task<bool> ExisteAtivoAsync(int barbeiroId, CancellationToken cancellationToken = default) =>
            _context.Barbeiros
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == barbeiroId &&
                    x.Usuario.Ativado,
                cancellationToken);

        public async Task AdicionarAsync(Barbeiro barbeiro, CancellationToken cancellationToken = default) =>
            await _context.Barbeiros.AddAsync(barbeiro, cancellationToken);

        public Task AtualizarAsync(Barbeiro barbeiro, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.Barbeiros.Update(barbeiro);
            return Task.CompletedTask;
        }
    }
}
