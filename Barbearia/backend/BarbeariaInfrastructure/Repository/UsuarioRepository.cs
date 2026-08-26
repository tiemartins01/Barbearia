using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public sealed class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Usuario?> ObterPorIdAsync(int usuarioId) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.Id == usuarioId);

        public Task<Usuario?> ObterPorLoginAsync(string login) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.Login == login);

        public Task<Usuario?> ObterPorEmailAsync(string email) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.Email.Valor == email);

        public Task<Usuario?> ObterPorCpfAsync(string cpf) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.CPF.Valor == cpf);

        public Task<Usuario?> ObterPorTelefoneAsync(string telefone) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.Numero.Valor == telefone);

        public async Task AdicionarAsync(Usuario usuario) =>
            await _context.Usuarios.AddAsync(usuario);

        public Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            return Task.CompletedTask;
        }
    }
}
