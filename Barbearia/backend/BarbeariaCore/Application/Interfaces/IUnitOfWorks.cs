namespace BarbeariaCore.Application.Interfaces
{
    public interface IUnitOfWork
    {

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();

    }
}
