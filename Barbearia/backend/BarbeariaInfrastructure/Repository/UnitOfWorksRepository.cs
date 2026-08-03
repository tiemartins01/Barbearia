using Barbearia.Core.Excepetion;
using Barbearia.Core.Infrastructure.Data;
using Barbearia.Core.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbearia.Core.Repository
{
    public class UnitOfWorksRepository : IUnitOfWork, IAsyncDisposable
    {

        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWorksRepository(AppDbContext context)
        {
            _context = context;

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
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation,
                    ConstraintName: "ux_horarios_barbeiro_horario_ativo"
                })
            {
                throw new DomainException(
                    "APPOINTMENT_TIME_CONFLICT",
                    "O barbeiro já possui um agendamento ativo neste horário.");
            }
        }
        
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
