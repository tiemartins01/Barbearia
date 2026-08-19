using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Infrastructure.Data;
using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
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
