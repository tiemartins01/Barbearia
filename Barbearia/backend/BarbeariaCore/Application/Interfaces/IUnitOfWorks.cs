namespace Barbearia.Core.Interface
{
    public interface IUnitOfWork
    {

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();

    }
}
