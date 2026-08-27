using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Exceptions;

namespace BarbeariaCore.UseCases.Security
{
    public sealed class RevogarSessao
    {

        private readonly IRefreshRepository _repository;
        private readonly IUnitOfWork _uow;

        public RevogarSessao(IRefreshRepository repository, IUnitOfWork uow)
        {
            _repository = repository;
            _uow = uow;
        }

        public async Task ExecutarAsync(int userId, int sessionId)
        {
            if (!await _repository.RevokeByIdAsync(sessionId, userId))
                throw new NotFoundException("SESSION_NOT_FOUND", "Sessão não encontrada.");

            await _uow.SaveChangesAsync();
        }

    }
}
