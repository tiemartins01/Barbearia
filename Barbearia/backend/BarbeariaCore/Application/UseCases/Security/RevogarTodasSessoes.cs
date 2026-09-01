using BarbeariaCore.Application.Interfaces;

namespace BarbeariaCore.UseCases.Security
{
    public sealed class RevogarTodasSessoes
    {
        private readonly IRefreshRepository _repository;
        private readonly IUnitOfWork _uow;

        public RevogarTodasSessoes(IRefreshRepository repository, IUnitOfWork uow)
        {
            _repository = repository;
            _uow = uow;
        }

        public async Task ExecutarAsync(int userId, CancellationToken cancellationToken = default)
        {
            await _repository.RevokeAllByUserAsync(userId, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
        }

    }
}
