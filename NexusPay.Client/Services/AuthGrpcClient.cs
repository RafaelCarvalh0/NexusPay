using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Client.Services.Interfaces;
using NexusPay.Contracts;
using NexusPay.Shared.Models.Auth;

namespace NexusPay.Client.Services
{
    public class AuthGrpcClient(GrpcChannel channel, ILogger<AuthGrpcClient> logger) : IAuthGrpcClient
    {
        private readonly AuthService.AuthServiceClient _client = new(channel);

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            LoginGrpcResponse response = await _client.LoginAsync(new LoginGrpcRequest
            {
                Email = request.Email,
                Password = request.Password
            });

            logger.LogInformation("User logged in: {Email}", request.Email);

            return new LoginResponse(
                Token: response.Token,
                TokenType: response.TokenType,
                ExpiresIn: response.ExpiresIn,
                UserId: response.UserId,
                UserName: response.UserName,
                Role: response.Role
            );
        }

        public async Task ForgotPasswordRequestAsync(ForgotPasswordRequest request)
        {
            await _client.ForgotPasswordAsync(new ForgotPasswordGrpcRequest
            {
                Email = request.Email
            });

            logger.LogInformation("Password reset requested for email: {Email}", request.Email);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            await _client.ResetPasswordAsync(new ResetPasswordGrpcRequest
            {
                Email = request.Email,
                Token = request.Token,
                NewPassword = request.NewPassword
            });

            logger.LogInformation("Password reset for email: {Email}", request.Email);
        }

        public async Task LogoutAsync(LogoutRequest request)
        {
            await _client.LogoutAsync(new LogoutGrpcRequest
            {
                Jti = request.Jti,
                UserId = request.UserId
            });

            logger.LogInformation("User logged out: {UserId}", request.UserId);
        }

        public async Task<bool> IsTokenRevokedAsync(string jti)
        {
            IsTokenRevokedGrpcResponse response = await _client.IsTokenRevokedAsync(
                new IsTokenRevokedGrpcRequest { Jti = jti });

            return response.IsRevoked;
        }
    }
}
