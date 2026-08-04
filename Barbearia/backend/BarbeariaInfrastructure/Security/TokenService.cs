using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Barbearia.Core.Service
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _iconfiguration;

        public TokenService(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
        }
        public string GenerateToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_iconfiguration["Jwt:Key"])
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
        new Claim(
            ClaimTypes.NameIdentifier,
            usuario.Id.ToString()
        ),

        new Claim(
            ClaimTypes.Name,
            usuario.Nome
        ),
        new Claim(
            ClaimTypes.Role,
            usuario.Role.ToString()
        ),
    };
            var token = new JwtSecurityToken(
                issuer: _iconfiguration["Jwt:Issuer"],
                audience: _iconfiguration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
        }
    }
}