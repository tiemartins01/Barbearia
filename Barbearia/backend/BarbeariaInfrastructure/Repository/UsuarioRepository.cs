using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace BarbeariaInfrastructure.Repository
{
    public sealed class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Usuario?> ObterPorIdAsync(int usuarioId, CancellationToken cancellationToken = default) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.Id == usuarioId, cancellationToken);

        public Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken cancellationToken = default) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);

        public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.Email.Valor == email, cancellationToken);

        public Task<Usuario?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.Cpf.Valor == cpf, cancellationToken);

        public Task<Usuario?> ObterPorTelefoneAsync(string telefone, CancellationToken cancellationToken = default) =>
            _context.Usuarios.FirstOrDefaultAsync(x => x.Telefone.Valor == telefone, cancellationToken);

        public async Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default) =>
            await _context.Usuarios.AddAsync(usuario, cancellationToken);

        public Task AtualizarAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.Usuarios.Update(usuario);
            return Task.CompletedTask;
        }
    }
}
