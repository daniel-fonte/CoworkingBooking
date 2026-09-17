using CoworkingBooking.Application;
using CoworkingBooking.Application.Workspace.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace CoworkingBooking.Api.Providers
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services
        ) {
            services.AddSingleton<WorkspaceAvailabilityRecurrenceMapper>();
            services.AddSingleton<WorkspaceAvailabilityMapper>();
            services.AddSingleton<WorkspaceMapper>();
            services.AddUseCases(typeof(ApplicationAssembly).Assembly);

            return services;
        }
    }
}