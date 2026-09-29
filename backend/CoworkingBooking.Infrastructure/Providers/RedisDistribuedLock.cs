using CoworkingBooking.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CoworkingBooking.Infraestructure.Providers
{
    public class RedisDistribuedLock : IDistribuedLock
    {
        private const string ReleaseScript = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

        private readonly RedisService _redisService;
        private readonly ILogger<RedisDistribuedLock> _logger;

        public RedisDistribuedLock(
            RedisService redisService,
            ILogger<RedisDistribuedLock> logger
        )
        {
            _redisService = redisService;
            _logger = logger;
        }

        public async Task<string?> AcquireLock(
            string resource,
            TimeSpan ttl,
            TimeSpan wait
        )
        {
            var token = new Guid().ToString();
            var key = $"lock:{resource}";

            var acquiredTimeout = DateTime.UtcNow + wait;

            do
            {
                if (await _redisService.GetDatabase().StringSetAsync(key, token, ttl, When.NotExists))
            {
                _logger.LogInformation("Lock acquired to {Resource}: Token {Token}", resource, token);
                return token;
            }
            } while (DateTime.UtcNow < acquiredTimeout);

            return null;
        }

        public async Task<bool> ReleaseAsync(string resource, string token)
        {
            var result = await _redisService.GetDatabase().ScriptEvaluateAsync(
                ReleaseScript,
                [$"lock:{resource}"],
                [token]);

            return (int)result == 1;
        }


    }
}