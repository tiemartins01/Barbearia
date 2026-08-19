using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Infrastructure.Data;
using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public class LoginRepository : ILoginRepository
    {

        private readonly AppDbContext _context;

        public LoginRepository(AppDbContext context)
        {
            _context = context;
        }

        public  Task<Usuario?> ObterPorLoginAsync(string login) => _context.Usuarios.FirstOrDefaultAsync(x => x.Login == login); // leitura e retorno das informações da pessoa!

        public Task Atualizar(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            return Task.CompletedTask;
        }

        public Task<Usuario?> ObterPorIdAsync(int id) => _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id); // leitura e retorno das informações da pessoa!

    }
}
