using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Domain.Enum;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Queries
{
    public sealed class BarbeirosQuery : IBarbeirosQuery
    {
        private readonly AppDbContext _context;

        public BarbeirosQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<DTOBarbeiro>> ListarAtivosAsync()
        {
            var barbeiros = await _context.Barbeiros
                .AsNoTracking()
                .Where(b =>
                    b.Usuario.Role == RolePerson.Barbeiro &&
                    b.Usuario.Ativado)
                .OrderBy(b => b.Usuario.Nome)
                .Select(b => new
                {
                    b.Id,
                    b.Usuario.Nome,
                    b.Especialidade,
                    NotaMedia = _context.Avaliacoes
                        .Where(a => a.BarbeiroId == b.Id)
                        .Average(a => (double?)a.Nota) ?? 0,
                    QuantidadeAvaliacoes = _context.Avaliacoes
                        .Count(a => a.BarbeiroId == b.Id)
                })
                .ToListAsync();

            return barbeiros
                .Select(x => new DTOBarbeiro
                {
                    Id = x.Id,
                    Nome = x.Nome,
                    Iniciais = GerarIniciais(x.Nome),
                    Especialidade = x.Especialidade,
                    NotaMedia = x.NotaMedia,
                    QuantidadeAvaliacoes = x.QuantidadeAvaliacoes
                })
                .ToList();
        }

        private static string GerarIniciais(string nome) =>
            string.Concat(
                nome.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .Select(p => char.ToUpperInvariant(p[0])));
    }
}
