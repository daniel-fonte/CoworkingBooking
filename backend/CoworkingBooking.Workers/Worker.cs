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

        var consumer = scope.ServiceProvider
            .GetRequiredService<UpdatedWorkspaceAvailabilityConsumer>();

        await consumer.InitializeAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await consumer.ConsumeAsync(stoppingToken);
        }
    }
}
