using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Contracts;
using NexusPay.Shared.Models.User;

namespace NexusPay.Client.Services
{
    public interface IUserGrpcClient
    {
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
    }

    public class UserGrpcClient : IUserGrpcClient
    {
        private readonly UserService.UserServiceClient _client;

        public UserGrpcClient(GrpcChannel channel, ILogger<UserGrpcClient> logger)
        {
            _client = new UserService.UserServiceClient(channel);
        }
        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            CreateUserGrpcResponse response = await _client.CreateUserAsync(new CreateUserGrpcRequest
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password
            });

            return new CreateUserResponse(Message: response.Message);
        }
    }
}
