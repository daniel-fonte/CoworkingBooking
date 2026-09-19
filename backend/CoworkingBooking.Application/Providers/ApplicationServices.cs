using CoworkingBooking.Application;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Application.Workspace.Ports;
using CoworkingBooking.Application.WorkspaceCalendar.Adapters;
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

            services.AddSingleton<IWorkspaceCalendarExistenceCheckerPort, WorkspaceCalendarExistenceCheckerAdapter>();
            
            services.AddUseCases(typeof(ApplicationAssembly).Assembly);

            return services;
        }
    }
}