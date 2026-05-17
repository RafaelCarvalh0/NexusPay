using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Contracts;
using NexusPay.Entities.Models.Auth;

namespace NexusPay.Client.Services
{
    public interface IAuthGrpcClient
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task LogoutAsync(LogoutRequest request);
    }

    public class AuthGrpcClient : IAuthGrpcClient
    {
        private readonly GrpcChannel _channel;
        private readonly AuthService.AuthServiceClient _client;

        public AuthGrpcClient(string baseUrl, ILogger<AuthGrpcClient> logger)
        {
            _channel = GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }
            });

            _client = new AuthService.AuthServiceClient(_channel);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            LoginGrpcResponse response = await _client.LoginAsync(new LoginGrpcRequest
            {
                Username = request.Username,
                Password = request.Password
            });

            return new LoginResponse
            {
                Token = response.Token,
                Message = response.Message
            };
        }

        public async Task LogoutAsync(LogoutRequest request)
        {

        }
    }
}
