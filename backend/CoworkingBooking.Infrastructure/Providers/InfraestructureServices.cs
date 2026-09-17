using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Infraestructure.Mappers;
using CoworkingBooking.Infraestructure.Repositories;
using CoworkingBooking.Shared.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoworkingBooking.Infraestructure.Providers
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

            services.AddSingleton<MongodbDatabaseService>();
            services.AddScoped<ITransactionManager, MongodbTransactionManagerService>();

            services.AddSingleton<WorkspaceAvailabilityRecurrencePersistenceMapper>();
            services.AddSingleton<WorkspaceAvailabilityPersistenceMapper>();
            services.AddSingleton<WorkspacePersistenceMapper>();
            services.AddSingleton<WorkspaceCalendarPersistenceMapper>();

            services.AddSingleton<IWorkspaceRepository, WorkspaceRepository>();
            services.AddSingleton<IWorkspaceCalendarRepository, WorkspaceCalendarRepository>();
            
            services.AddMongoMigrations(
                typeof(InfrastructureAssembly).Assembly
            );

            services.AddPublishers(
                typeof(InfrastructureAssembly).Assembly
            );

            return services;
        }
    }
}