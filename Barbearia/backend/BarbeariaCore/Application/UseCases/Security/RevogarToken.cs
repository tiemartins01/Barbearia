using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Exceptions;

namespace BarbeariaCore.UseCases.Security
{
    public sealed class RevogarToken
    {
        private readonly IRefreshRepository _repository;
        private readonly IUnitOfWork _uow;

        public RevogarToken(IRefreshRepository repository, IUnitOfWork uow)
        {
            _uow = uow;
            _repository = repository;
        }
        public async Task ExecutarAsync(string refreshToken)
        {
            var token = await _repository.GetAsync(refreshToken);
            if (token is null || token.Revogado)
                throw new AuthenticationException("INVALID_REFRESH",
    "Credenciais inválidas.");

            await _repository.RevokeAsync(refreshToken, null, "LOGOUT");
            await _uow.SaveChangesAsync();
        }
    }
}
