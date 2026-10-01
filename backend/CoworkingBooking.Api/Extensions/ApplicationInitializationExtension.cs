using CoworkingBooking.Infraestructure.DependencyInjection;
using CoworkingBooking.Infraestructure.Providers;
using Serilog;

namespace CoworkingBooking.Api.Extensions
{
    public static class ApplicationInitializationExtensions
    {
        public static async Task InitializeApplicationAsync(
            this WebApplication app
        )
        {
            await app.InitializeMongoAsync();
            await app.InitializeRedisAsync();
            await app.RunMigrationsAsync();
            await app.InitializeQueuesAsync();
        }

        private static async Task InitializeMongoAsync(
            this WebApplication app
        )
        {
            app.Services.GetRequiredService<MongodbDatabaseService>();
        }

        private static async Task InitializeRedisAsync(
            this WebApplication app
        )
        {
            using var scope = app.Services.CreateScope();

            var redisService = scope.ServiceProvider
                .GetRequiredService<RedisService>();

            try
            {
                await redisService.ValidateConnectionAsync(
                    5,
                    TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to connect to Redis");
                throw;
            }
        }

        private static async Task RunMigrationsAsync(
            this WebApplication app
        )
        {
            using var scope = app.Services.CreateScope();

            var migrationRunner = scope.ServiceProvider
                .GetRequiredService<MigrationRunner>();

            await migrationRunner.RunMigrations();
        }

        private static async Task InitializeQueuesAsync(
            this WebApplication app
        )
        {
            using var scope = app.Services.CreateScope();

            var publishConnectionService = scope.ServiceProvider
                .GetRequiredService<PublishConnectionService>();

            await publishConnectionService.GetQueuesUrl();
        }
    }
}