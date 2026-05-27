using StackExchange.Redis;

namespace NexusPay.Server.Helper.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SetStringAsync(string key, string value, TimeSpan expiration)
            => await _db.StringSetAsync(key, value, expiration);

        public async Task<string?> GetStringAsync(string key)
            => await _db.StringGetAsync(key);

        public async Task<bool> DeleteKeyAsync(string key)
            => await _db.KeyDeleteAsync(key);

        // Saves the JTI as the user's active session
        public async Task SaveActiveSessionAsync(string userId, string jti, TimeSpan expiration)
            => await _db.StringSetAsync($"active_session:{userId}", jti, expiration);

        // Returns the JTI of the user's current active session
        public async Task<string?> GetActiveSessionAsync(string userId)
            => await _db.StringGetAsync($"active_session:{userId}");

        // Adds the JTI to the blacklist
        public async Task RevokeTokenAsync(string jti, TimeSpan expiration)
            => await _db.StringSetAsync($"revoked:{jti}", "1", expiration);

        // Checks whether the JTI is blacklisted
        public async Task<bool> IsTokenRevokedAsync(string jti)
            => await _db.KeyExistsAsync($"revoked:{jti}");

        // Removes the user's active session
        public async Task RemoveActiveSessionAsync(string userId)
            => await _db.KeyDeleteAsync($"active_session:{userId}");
    }
}
