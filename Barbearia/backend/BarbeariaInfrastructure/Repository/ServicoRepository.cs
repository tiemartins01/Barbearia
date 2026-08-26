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

        public Task<Servico?> ObterPorIdAsync(int servicoId) =>
            _context.Servicos.FirstOrDefaultAsync(x => x.Id == servicoId);

        public Task<Servico?> ObterAtivoPorIdAsync(int servicoId) =>
            _context.Servicos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == servicoId && x.Ativo);

        public Task<bool> ExisteAtivoAsync(int servicoId) =>
            _context.Servicos
                .AsNoTracking()
                .AnyAsync(x => x.Id == servicoId && x.Ativo);

        public async Task AdicionarAsync(Servico servico) =>
            await _context.Servicos.AddAsync(servico);

        public Task AtualizarAsync(Servico servico)
        {
            _context.Servicos.Update(servico);
            return Task.CompletedTask;
        }
    }
}
