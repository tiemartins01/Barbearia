using Barbearia.Core.Exceptions;
using Barbearia.Core.Infrastructure.Data;
using Barbearia.Core.Interface;
using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace Barbearia.Core.Repository
{
    public class UnitOfWorksRepository : IUnitOfWork, IAsyncDisposable
    {

        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly IDatabaseErrorClassifier _databaseError;

        public UnitOfWorksRepository(AppDbContext context, IDatabaseErrorClassifier databaseError)
        {
            _context = context;
            _databaseError = databaseError;
        }
        // INICIA UMA TRANSAÇÃO
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new DomainException("Já existe uma transação ativa!");

            _transaction = await _context.Database.BeginTransactionAsync();
        }
        // REALIZA O COMMIT
        public async Task CommitTransactionAsync()
        {
            if (_transaction is null)
                return;
            
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
            
                
        }
        // NÃO SALVA NADA POR ALGUM ERRO
        public async Task RollbackAsync()
        {
            if (_transaction is null)
                return;

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        // SALVA AS MUDANÇAS
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
                when (_databaseError.IsUniqueViolation(
                    ex,
                     "ux_horarios_barbeiro_horario_ativo"))
            {
                throw new DomainException(
                    "APPOINTMENT_TIME_CONFLICT",
                    "O barbeiro já possui um agendamento ativo neste horário.");
            }
        }

        //public async Task SaveChangesAsync()
        //{
        //    Console.WriteLine("===== SAVE CHANGES =====");

        //    Console.WriteLine(
        //        $"Transação ativa: {_context.Database.CurrentTransaction != null}");

        //    foreach (var entry in _context.ChangeTracker.Entries())
        //    {
        //        Console.WriteLine(
        //            $"{entry.Entity.GetType().Name} -> {entry.State}");
        //    }

        //    var resultado = await _context.SaveChangesAsync();

        //    Console.WriteLine($"SALVOS: {resultado}");
        //}

        public async ValueTask DisposeAsync()
        {
            if ( _transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
