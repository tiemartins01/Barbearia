using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Domain.ValueObjects;
using Barbearia.Core.Infrastructure.Data;
using Barbearia.Core.Interface;
using Microsoft.EntityFrameworkCore;

namespace Barbearia.Core.Repository
{
    public class EmailEsqueciSenhaRepository : IEmailEsqueciSenhaRepository
    {

        private readonly AppDbContext _context;

        public EmailEsqueciSenhaRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task<Usuario?> BuscarUsuarioPorEmailAsync(string email) => _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Email.EmailPessoa == email);
        public Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            return Task.CompletedTask; // COMO JÁ VAI SER SALVO NO SERVICE, APENAS TERMINA A TAREFA
        }
    }
}
