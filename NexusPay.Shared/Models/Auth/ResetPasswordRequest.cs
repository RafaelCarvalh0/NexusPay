namespace NexusPay.Shared.Models.Auth
{
    public record ResetPasswordRequest(string Email, string Token, string NewPassword);
}
