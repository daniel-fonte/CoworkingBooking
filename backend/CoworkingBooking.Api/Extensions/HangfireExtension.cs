using CoworkingBooking.Infraestructure.CronJobs;
using Hangfire;
using Hangfire.Redis.StackExchange;

namespace CoworkingBooking.Api.Extensions
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfireServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddHangfire(config =>
            {
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseRedisStorage(
                        configuration.GetSection("RedisSettings")["ConnectionString"]!,
                        new RedisStorageOptions
                        {
                            Prefix = "hangfire:"
                        });
            });

            services.AddHangfireServer();

            return services;
        }

        public static void AddRecurringJobs()
        {
            RecurringJob.AddOrUpdate<CleanWorkspaceCalendarRecurrencesCronJob>(
                "clean-workspace-calendar-recurrences",
                job => job.ExecuteAsync(),
                "0 23 * * *"
            );
        }
    }
}