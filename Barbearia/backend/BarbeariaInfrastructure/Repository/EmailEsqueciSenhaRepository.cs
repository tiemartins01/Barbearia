using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Infrastructure.Data;
using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public class EmailEsqueciSenhaRepository : IEmailEsqueciSenhaRepository
    {

        private readonly AppDbContext _context;

        public EmailEsqueciSenhaRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task<Usuario?> BuscarUsuarioPorEmailAsync(string email) => _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Email.Valor == email);
        public Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            return Task.CompletedTask; // COMO JÁ VAI SER SALVO NO SERVICE, APENAS TERMINA A TAREFA
        }
    }
}
