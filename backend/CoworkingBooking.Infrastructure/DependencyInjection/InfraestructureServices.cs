using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Infraestructure.Providers;
using CoworkingBooking.Infraestructure.Repositories;
using CoworkingBooking.Shared.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CoworkingBooking.Infraestructure.DependencyInjection
{
    public static class InfraestructureServices
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration
        ) {
            services.Configure<MongoDbSettings>(
                configuration.GetSection("MongoDbSettings")
            );

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(configuration.GetSection("RedisSettings")["ConnectionString"]!)
            );

            services.AddSingleton<MongodbDatabaseService>();
            services.AddSingleton<RedisService>();

            services.AddScoped<ITransactionManager, MongodbTransactionManagerService>();
            services.AddSingleton<IDistribuedLock, RedisDistribuedLock>();

            services.AddInfraestructureMappersService();
            services.AddRepositoriesService(
                typeof(InfrastructureAssembly).Assembly
            );

            services.AddSingleton<IWorkspaceRepository, WorkspaceRepository>();
            services.AddSingleton<IWorkspaceCalendarRepository, WorkspaceCalendarRepository>();
            
            services.AddMongoMigrations(
                typeof(InfrastructureAssembly).Assembly
            );

            services.AddPublishers(
                typeof(InfrastructureAssembly).Assembly
            );

            services.AddSingleton(typeof(ICacheRepository<>), typeof(RedisCacheRepository<>));

            return services;
        }
    }
}