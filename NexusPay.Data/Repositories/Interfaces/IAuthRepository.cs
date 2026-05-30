using NexusPay.Shared.Models.Auth;
using NexusPay.Shared.Models.Auth.Claims;

namespace NexusPay.Data.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<AuthClaims> Login(LoginRequest request);
        Task ResetPassword(ResetPasswordRequest resetPasswordRequest);
    }
}
