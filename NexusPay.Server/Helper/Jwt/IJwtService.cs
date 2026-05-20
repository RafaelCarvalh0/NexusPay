using NexusPay.Shared.Models.Auth.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace NexusPay.Server.Helper.Jwt
{
    public interface IJwtService
    {
        JwtSecurityToken GenerateToken(AuthClaims claims);
        string WriteToken(JwtSecurityToken jwt);
    }
}
