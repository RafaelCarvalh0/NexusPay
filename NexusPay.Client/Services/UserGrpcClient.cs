using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Contracts;
using NexusPay.Shared.Models.User;

namespace NexusPay.Client.Services
{
    public interface IUserGrpcClient
    {
        Task CreateUserAsync(CreateUserRequest request);
        Task UpdateUserAsync(string id, UpdateUserRequest request);
    }

    public class UserGrpcClient : IUserGrpcClient
    {
        private readonly UserService.UserServiceClient _client;

        public UserGrpcClient(GrpcChannel channel, ILogger<UserGrpcClient> logger)
        {
            _client = new UserService.UserServiceClient(channel);
        }
        public async Task CreateUserAsync(CreateUserRequest request)
        {
            await _client.CreateUserAsync(new CreateUserGrpcRequest
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                RoleId = request.RoleId
            });
        }

        public async Task UpdateUserAsync(string id, UpdateUserRequest request)
        {
            await _client.UpdateUserAsync(new UpdateUserGrpcRequest
            {
                Id = id,
                Name = request.Name,
                Email = request.Email
            });
        }
    }
}
