using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Contracts;
using NexusPay.Shared.Models.Auth;

namespace NexusPay.Client.Services
{
    public interface IAuthGrpcClient
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LogoutResponse> LogoutAsync(LogoutRequest request);
        Task<bool> IsTokenRevokedAsync(string jti);
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

        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            LogoutGrpcResponse response = await _client.LogoutAsync(new LogoutGrpcRequest
            {
                Jti = request.Jti,
                UserId = request.UserId
            });

            return new LogoutResponse(response.Message);
        }

        public async Task<bool> IsTokenRevokedAsync(string jti)
        {
            IsTokenRevokedGrpcResponse response = await _client.IsTokenRevokedAsync(
                new IsTokenRevokedGrpcRequest { Jti = jti });

            return response.IsRevoked;
        }
    }
}
