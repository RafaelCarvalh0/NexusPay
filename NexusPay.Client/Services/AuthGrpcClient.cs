using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Contracts;
using NexusPay.Shared.Models.Auth;

namespace NexusPay.Client.Services
{
    public interface IAuthGrpcClient
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task ForgotPasswordRequestAsync(ForgotPasswordRequest request);
        Task LogoutAsync(LogoutRequest request);
        Task<bool> IsTokenRevokedAsync(string jti);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }

    public class AuthGrpcClient : IAuthGrpcClient
    {
        private readonly AuthService.AuthServiceClient _client;

        public AuthGrpcClient(GrpcChannel channel, ILogger<AuthGrpcClient> logger)
        {
            _client = new AuthService.AuthServiceClient(channel);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            LoginGrpcResponse response = await _client.LoginAsync(new LoginGrpcRequest
            {
                Email = request.Email,
                Password = request.Password
            });

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
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            await _client.ResetPasswordAsync(new ResetPasswordGrpcRequest
            {
                Email = request.Email,
                Token = request.Token,
                NewPassword = request.NewPassword
            });
        }

        public async Task LogoutAsync(LogoutRequest request)
        {
            await _client.LogoutAsync(new LogoutGrpcRequest
            {
                Jti = request.Jti,
                UserId = request.UserId
            });
        }

        public async Task<bool> IsTokenRevokedAsync(string jti)
        {
            IsTokenRevokedGrpcResponse response = await _client.IsTokenRevokedAsync(
                new IsTokenRevokedGrpcRequest { Jti = jti });

            return response.IsRevoked;
        }
    }
}
