namespace NexusPay.Server.Helper.Redis
{
    public interface IRedisService
    {
        Task SetStringAsync(string key, string value, TimeSpan expiration);
        Task<string?> GetStringAsync(string key);
        Task<bool> DeleteKeyAsync(string key);

        Task SaveActiveSessionAsync(string userId, string jti, TimeSpan expiration);
        Task<string?> GetActiveSessionAsync(string userId);
        Task RevokeTokenAsync(string jti, TimeSpan expiration);
        Task<bool> IsTokenRevokedAsync(string jti);
        Task RemoveActiveSessionAsync(string userId);
    }
}
