using System.Text.Json;
using CoworkingBooking.Infraestructure.Providers;
using CoworkingBooking.Infraestructure.Publishers;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Events;
using CoworkingBooking.Shared.Interfaces;
using CoworkingBooking.Shared.Publishers;
using StackExchange.Redis;

namespace CoworkingBooking.Infraestructure.Repositories
{
    public class RedisCacheRepository<T> : ICacheRepository<T>
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _database;
        private readonly IRefreshCachePublisher _refreshCachePublish;

        public RedisCacheRepository(
            RedisService redisService,
            IRefreshCachePublisher refreshCachePublish
        )
        {
            _redisService = redisService;
            _database = _redisService.GetDatabase();
            _refreshCachePublish = refreshCachePublish;
        }

        public async Task<T?> GetByKey(string key, Func<string, Task<T?>> resolveDataFunction)
        {
            var cacheFound = await _database.StringGetAsync(key);

            if (cacheFound.HasValue)
            {
                var cached = JsonSerializer.Deserialize<CacheEntry<T>>(cacheFound.ToString());

                if (cached is null)
                {
                    return default;
                }

                var createdAtElapsed = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - cached.CreatedAt;

                if (createdAtElapsed > 3600)
                {
                    await _refreshCachePublish.EnqueueMessage(
                        new RefreshCacheEvent(typeof(T).ToString(), key)
                    );
                }

                return cached.Data;
            }

            var keyValueIndentifier = key.Split(':')[1];

            var dataFromDB = await resolveDataFunction(keyValueIndentifier);
            
            if (dataFromDB is null)
            {
                return default;
            }

            var cacheEntry = new CacheEntry<T>
            {
                Data = dataFromDB,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var dataJson = JsonSerializer.Serialize(cacheEntry);

            await _database.StringSetAsync(key, dataJson, TimeSpan.FromHours(1));

            return dataFromDB;
        }

        public async Task UpdateByKey(string key, string data)
        {
            await _database.StringSetAsync(key, data, TimeSpan.FromHours(1), When.Exists);
        }
    }
}