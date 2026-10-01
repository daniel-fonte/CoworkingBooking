using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using CoworkingBooking.Api.HealthChecks;

namespace CoworkingBooking.Api.DependencyInjection
{
    public static class HealthChecksService
    {
        public static IServiceCollection AddHealthCheckServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddHealthChecks()
                .AddCheck<RedisHealthCheck>("redis", tags: ["ready"])
                .AddCheck<MongoDBHealthCheck>("mongoDB", tags: ["ready"]);

            return services;
        }
    }
}