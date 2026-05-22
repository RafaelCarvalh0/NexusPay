namespace NexusPay.Shared.Models.User
{
    public record CreateUserRequest(string Name, string Email, string Password, int RoleId);
}
    