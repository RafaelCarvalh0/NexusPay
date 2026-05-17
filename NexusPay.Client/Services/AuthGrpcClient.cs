using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Contracts;
using NexusPay.Shared.Models.Auth;

namespace NexusPay.Client.Services
{
    public interface IAuthGrpcClient
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task LogoutAsync(LogoutRequest request);
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

            return new LoginResponse(Token: response.Token, Message: response.Message);
        }

        public async Task LogoutAsync(LogoutRequest request)
        {

        }
    }
}
