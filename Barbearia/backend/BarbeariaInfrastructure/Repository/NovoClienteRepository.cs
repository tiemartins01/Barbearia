using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Infrastructure.Data;
using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public class NovoClienteRepository : INovoClienteRepository
    {

        private readonly AppDbContext _context;

        public NovoClienteRepository(AppDbContext context)
        {
            _context = context;
        }
        // Verifica se existe alguma informação já criada no banco de dados e faz com que não seja possível adicionar novamente
        public Task<Usuario?> VerificarDuplicidadeAsync(string email, string cpf, string telefone,string login) =>  
            _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x =>
                x.CPF.Numero == cpf ||
                x.Email.EmailPessoa == email ||
                x.Phone.Telefone == telefone ||
                x.Login == login); // verificar sobre ter ativado para procurar menos
        // ADICIONANDO NOVO CLIENTE
        public async Task CadastraNovoClienteAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }
    }
}
