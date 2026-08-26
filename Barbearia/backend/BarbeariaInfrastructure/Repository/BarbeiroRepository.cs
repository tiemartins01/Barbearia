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

        public Task<Barbeiro?> ObterPorIdAsync(int barbeiroId) =>
            _context.Barbeiros.FirstOrDefaultAsync(x => x.Id == barbeiroId);

        public Task<bool> ExisteAtivoAsync(int barbeiroId) =>
            _context.Barbeiros
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == barbeiroId &&
                    x.Usuario.Ativado);

        public async Task AdicionarAsync(Barbeiro barbeiro) =>
            await _context.Barbeiros.AddAsync(barbeiro);

        public Task AtualizarAsync(Barbeiro barbeiro)
        {
            _context.Barbeiros.Update(barbeiro);
            return Task.CompletedTask;
        }
    }
}
