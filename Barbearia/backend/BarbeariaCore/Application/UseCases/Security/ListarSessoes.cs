using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;

namespace BarbeariaCore.UseCases.Security
{
    public sealed class ListarSessoes
    {
        private readonly IRefreshRepository _repository;

        public ListarSessoes(IRefreshRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<DTOSessao>> ExecutarAsync(int userId, string? currentToken, CancellationToken cancellationToken)
        {
            var sessions = await _repository.ListByUserAsync(userId, cancellationToken);
            return sessions.Select(x => new DTOSessao
            {
                Id = x.Id,
                CriadoEm = x.CriadoEm,
                ExpiraEm = x.ExpiraEm,
                Revogado = x.Revogado,
                Atual = !string.IsNullOrWhiteSpace(currentToken) && x.Token == currentToken
            }).ToList();
        }

    }
}
