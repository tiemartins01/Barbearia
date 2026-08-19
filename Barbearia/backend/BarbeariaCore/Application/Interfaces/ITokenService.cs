using BarbeariaCore.Domain.Entities;

namespace BarbeariaCore.Application.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(Usuario usuario);

        public string GenerateRefreshToken();

    }
}