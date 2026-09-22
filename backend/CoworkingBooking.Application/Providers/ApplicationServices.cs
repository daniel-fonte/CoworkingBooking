using CoworkingBooking.Application;
using CoworkingBooking.Application.Workspace.Adapters;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Application.Workspace.Ports;
using CoworkingBooking.Application.WorkspaceCalendar.Adapters;
using CoworkingBooking.Application.WorkspaceCalendar.Mappers;
using CoworkingBooking.Application.WorkspaceCalendar.Ports;
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
            services.AddSingleton<WorkspaceCalendarBookingMapper>();

            services.AddSingleton<IWorkspaceCalendarExistenceCheckerPort, WorkspaceCalendarExistenceCheckerAdapter>();
            services.AddSingleton<IGetWorkspaceAvailabilityTimezonePort, GetWorkspaceAvailabilityTimezoneAdapter>();
            services.AddSingleton<IWorkspaceExistenceCheckerPort, WorkspaceExistenceCheckerAdapter>();
            
            services.AddUseCases(typeof(ApplicationAssembly).Assembly);

            return services;
        }
    }
}