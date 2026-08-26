using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Queries
{
    public sealed class ServicosAtivosQuery : IServicosAtivosQuery
    {
        private readonly AppDbContext _context;

        public ServicosAtivosQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<DTOServicosAtivos>> ListarAsync()
        {
            return await _context.Servicos
                .AsNoTracking()
                .Where(x => x.Ativo)
                .OrderBy(x => x.Nome)
                .Select(x => new DTOServicosAtivos
                {
                    Id = x.Id,
                    NomeServico = x.Nome,
                    Duracao = x.Duracao,
                    Preco = x.Preco
                })
                .ToListAsync();
        }
    }
}
