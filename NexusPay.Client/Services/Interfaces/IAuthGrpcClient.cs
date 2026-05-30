using NexusPay.Shared.Models.Auth;

namespace NexusPay.Client.Services.Interfaces
{
    public interface IAuthGrpcClient
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task ForgotPasswordRequestAsync(ForgotPasswordRequest request);
        Task LogoutAsync(LogoutRequest request);
        Task<bool> IsTokenRevokedAsync(string jti);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}
