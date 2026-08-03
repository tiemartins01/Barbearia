using Barbearia.Core.Domain.Entities;
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
    public class TrocaSenhaRepository : ITrocaSenhaRepository
    {

        private readonly AppDbContext _context;

        public TrocaSenhaRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Usuario?> PegaInformacaoUsuario(string email) => _context.Usuarios.FirstOrDefaultAsync(x => x.Email.EmailPessoa == email);

        public async Task AtualizaUsuario(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await Task.CompletedTask;
        }

    }
}
