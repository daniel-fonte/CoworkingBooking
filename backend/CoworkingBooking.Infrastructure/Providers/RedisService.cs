using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CoworkingBooking.Infraestructure.Providers
{
    public class RedisService
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connection;
        private readonly ILogger<RedisService> logger;
        
        public RedisService(
            IConnectionMultiplexer connection,
            ILogger<RedisService> logger
        )
        {
            this.logger = logger;
            _connection = connection;
            _database = _connection.GetDatabase();
        }

        public async Task ValidateConnectionAsync(
            int maxRetries = 5,
            TimeSpan? initialDelay = null
        )
        {
            if (_connection.IsConnected)
            {
                var delay = initialDelay ?? TimeSpan.FromSeconds(1);

                Exception? lastException = null;

                for (var attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        var latency = await _database.PingAsync();

                        logger.LogInformation("Redis connection established successfully for database {DatabaseName}", _database.Database.ToString());
                        logger.LogInformation("Redis PING/PONG latency - {Latency}", latency);

                        return;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;

                        if (attempt == maxRetries)
                            break;

                        await Task.Delay(delay);

                        delay *= 2;
                    }
                }

                throw new InvalidOperationException($"Redis connection failed after {maxRetries} retries", lastException);
            }
           
        }
    
        public IDatabase GetDatabase()
        {
            return _database;
        }
    }
}