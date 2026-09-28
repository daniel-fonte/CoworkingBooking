using StackExchange.Redis;

namespace CoworkingBooking.Api.Providers
{
    public class RedisMonitorService : IHostedService
    {
        private readonly ILogger<RedisMonitorService> logger;
        private readonly  IConnectionMultiplexer mux;

        public RedisMonitorService(
            ILogger<RedisMonitorService> logger,
            IConnectionMultiplexer mux
        )
        {
            this.logger = logger;
            this.mux = mux;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mux.ConnectionFailed += OnConnectionFailed;
            mux.ConnectionRestored += OnConnectionRestored;
            mux.ErrorMessage += OnErrorMessage;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mux.ConnectionFailed -= OnConnectionFailed;
            mux.ConnectionRestored -= OnConnectionRestored;
            mux.ErrorMessage -= OnErrorMessage;
            return Task.CompletedTask;;
        }

        private void OnConnectionFailed(object? _, ConnectionFailedEventArgs e) =>
            logger.LogError(e.Exception, "Redis caiu: {Endpoint} ({Type}) - {Failure}",
                e.EndPoint, e.ConnectionType, e.FailureType);

        private void OnConnectionRestored(object? _, ConnectionFailedEventArgs e) =>
            logger.LogInformation("Redis restaurado: {Endpoint} ({Type})",
                e.EndPoint, e.ConnectionType);

        private void OnErrorMessage(object? _, RedisErrorEventArgs e) =>
            logger.LogWarning("Erro do Redis {Endpoint}: {Message}", e.EndPoint, e.Message);
    }
}