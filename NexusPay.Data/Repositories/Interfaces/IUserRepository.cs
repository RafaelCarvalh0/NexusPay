using NexusPay.Shared.Models.User;

namespace NexusPay.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task CreateUser(CreateUserRequest request);
        Task DeleteUser(Guid id);
        Task UpdateUser(string id, UpdateUserRequest request);
    }
}
