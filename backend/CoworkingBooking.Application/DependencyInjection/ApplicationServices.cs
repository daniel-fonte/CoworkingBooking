using CoworkingBooking.Application;
using CoworkingBooking.Application.DependencyInjection;
using CoworkingBooking.Application.Workspace.Adapters;
using CoworkingBooking.Application.Workspace.Ports;
using CoworkingBooking.Application.WorkspaceCalendar.Adapters;
using CoworkingBooking.Application.WorkspaceCalendar.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace CoworkingBooking.Api.Providers
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services
        ) {
            services.AddApplicationMappersServices();

            services.AddSingleton<IWorkspaceCalendarExistenceCheckerPort, WorkspaceCalendarExistenceCheckerAdapter>();
            services.AddSingleton<IGetWorkspaceAvailabilityTimezonePort, GetWorkspaceAvailabilityTimezoneAdapter>();
            services.AddSingleton<IWorkspaceExistenceCheckerPort, WorkspaceExistenceCheckerAdapter>();
            
            services.AddUseCases(typeof(ApplicationAssembly).Assembly);

            return services;
        }
    }
}