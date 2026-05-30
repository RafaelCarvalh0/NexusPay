using NexusPay.Shared.Models.User;

namespace NexusPay.Client.Services.Interfaces
{
    public interface IUserGrpcClient
    {
        Task CreateUserAsync(CreateUserRequest request);
        Task DeleteUserAsync(Guid userId);
        Task UpdateUserAsync(string id, UpdateUserRequest request);
    }
}
