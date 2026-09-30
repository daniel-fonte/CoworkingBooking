using CoworkingBooking.Workers.Consumers;

namespace CoworkingBooking.Workers;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<Worker> logger;

    public Worker(
        IServiceScopeFactory scopeFactory,
        ILogger<Worker> logger
    )
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();

        var updatedWorkspaceAvailabilityConsumer = scope.ServiceProvider
            .GetRequiredService<UpdatedWorkspaceAvailabilityConsumer>();

        var refreshCacheConsumer = scope.ServiceProvider
            .GetRequiredService<RefreshCacheConsumer>();

        await updatedWorkspaceAvailabilityConsumer.InitializeAsync(stoppingToken);
        await refreshCacheConsumer.InitializeAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await updatedWorkspaceAvailabilityConsumer.ConsumeAsync(stoppingToken);
            await refreshCacheConsumer.ConsumeAsync(stoppingToken);
        }
    }
}
