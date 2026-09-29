using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace CoworkingBooking.Api.HealthChecks
{
    public sealed class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _connection;
        
        public RedisHealthCheck(
            IConnectionMultiplexer connection
        )
        {
            _connection = connection;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (!_connection.IsConnected)
                {
                    return HealthCheckResult.Unhealthy("Redis unvaible");
                }

                var latency = await _connection.GetDatabase().PingAsync();

                return latency.TotalMilliseconds > 5000
                    ? HealthCheckResult.Degraded($"Redis slow: {latency.TotalMilliseconds:F0} ms")
                    : HealthCheckResult.Healthy($"Redis responded in {latency.TotalMilliseconds:F0} ms");
                }
            catch (System.Exception ex)
            {
                return HealthCheckResult.Unhealthy("Redis unexpected error", ex);
            }
        }
    }
}