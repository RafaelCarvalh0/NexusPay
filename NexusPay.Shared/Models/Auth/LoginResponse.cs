namespace NexusPay.Shared.Models.Auth
{
    public record LoginResponse(string Token, string TokenType, int ExpiresIn, string UserId, string UserName, string Role);
}
