using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Application.WorkspaceCalendar.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace CoworkingBooking.Application.DependencyInjection
{
    public static class MappersServices
    {
        public static IServiceCollection AddApplicationMappersServices(
            this IServiceCollection services
        )
        {
            services.AddSingleton<WorkspaceAvailabilityMapper>();
            services.AddSingleton<WorkspaceAvailabilityRecurrenceMapper>();
            
            services.AddSingleton<WorkspaceMapper>();
            services.AddSingleton<WorkspaceCalendarBookingMapper>();

            return services;
        }
    }
}