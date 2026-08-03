using Barbearia.Core.DTO;
using Barbearia.Core.Infrastructure.Data;
using Barbearia.Core.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbearia.Core.Repository
{
    public class ServicosAtivosRepository : IServicosRepository
    {

        private readonly AppDbContext _context;

        public ServicosAtivosRepository(AppDbContext context)
        {
            _context = context;
        }
        // PEGA TODOS OS SERVIÇOS ATIVOS
        public  Task<List<DTOServicosAtivos>> GetServicosAtivos() => _context.Servicos.AsNoTracking().Where(x => x.Ativo).OrderBy(x => x.Nome)
            .Select(x => new DTOServicosAtivos
            {
                Id = x.Id,
                NomeServico = x.Nome,
                Duracao = x.Duracao,
                Preco = x.Preco
            }).ToListAsync();
    }
}
