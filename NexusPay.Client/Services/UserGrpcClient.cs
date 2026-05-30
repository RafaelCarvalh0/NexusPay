using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Client.Services.Interfaces;
using NexusPay.Contracts;
using NexusPay.Shared.Models.User;

namespace NexusPay.Client.Services
{
    public class UserGrpcClient(GrpcChannel channel, ILogger<UserGrpcClient> logger) : IUserGrpcClient
    {
        private readonly UserService.UserServiceClient _client = new(channel);

        public async Task CreateUserAsync(CreateUserRequest request)
        {
            await _client.CreateUserAsync(new CreateUserGrpcRequest
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                RoleId = request.RoleId
            });

            logger.LogInformation("User created: {Email}", request.Email);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            await _client.DeleteUserAsync(new DeleteUserGrpcRequest
            {
                Id = userId.ToString()
            });

            logger.LogInformation("User deleted: {UserId}", userId);
        }

        public async Task UpdateUserAsync(string id, UpdateUserRequest request)
        {
            await _client.UpdateUserAsync(new UpdateUserGrpcRequest
            {
                Id = id,
                Name = request.Name,
                Email = request.Email
            });

            logger.LogInformation("User updated: {UserId}", id);
        }
    }
}
