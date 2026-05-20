using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NexusPay.Shared.Models.Auth.Claims;
using NexusPay.Shared.Models.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NexusPay.Server.Helper.Jwt
{
    public class JwtService : IJwtService
    {
        private static readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly JwtSettings _settings;

        public JwtService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public JwtSecurityToken GenerateToken(AuthClaims authClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, authClaims.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, authClaims.Email),
                new Claim(ClaimTypes.Role, authClaims.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            return new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
                signingCredentials: credentials
            );
        }

        public string WriteToken(JwtSecurityToken jwt) => _tokenHandler.WriteToken(jwt);
    }
}
