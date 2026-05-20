namespace NexusPay.Server.Helper.Redis
{
    public interface IRedisService
    {
        Task SaveActiveSessionAsync(string userId, string jti, TimeSpan expiration);
        Task<string?> GetActiveSessionAsync(string userId);
        Task RevokeTokenAsync(string jti, TimeSpan expiration);
        Task<bool> IsTokenRevokedAsync(string jti);
        Task RemoveActiveSessionAsync(string userId);
    }
}
