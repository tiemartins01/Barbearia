using Barbearia.Core.Domain.Entities;

namespace Barbearia.Core.Interface
{
    public interface ITokenService
    {
        public string GenerateToken(Usuario usuario);

        public string GenerateRefreshToken();

    }
}