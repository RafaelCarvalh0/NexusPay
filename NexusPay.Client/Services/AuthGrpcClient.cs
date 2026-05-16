using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Entities.Models.Auth;
using System.Threading.Channels;

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
        private readonly Contracts.Auth.AuthService.AuthServiceClient _client;
        private readonly ILogger<AuthGrpcClient> _logger;

        public AuthGrpcClient(string baseUrl, ILogger<AuthGrpcClient> logger)
        {
            _channel = GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }
            });

            _logger = logger;
            _client = new Contracts.Auth.AuthService.AuthServiceClient(_channel);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                Contracts.Auth.LoginResponse response = await _client.LoginAsync(new Contracts.Auth.LoginRequest
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
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                throw new Exception(ex.Status.Detail);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                throw new Exception("User not found.");
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Error in gRPC");
                throw;
            }
        }

        public async Task LogoutAsync(LogoutRequest request)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
