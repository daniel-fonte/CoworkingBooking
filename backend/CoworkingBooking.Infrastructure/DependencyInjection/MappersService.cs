using CoworkingBooking.Infraestructure.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace CoworkingBooking.Infraestructure.DependencyInjection
{
    public static class MappersService
    {
        public static IServiceCollection AddInfraestructureMappersService(
            this IServiceCollection services
        )
        {
            services.AddSingleton<WorkspaceAvailabilityRecurrencePersistenceMapper>();
            services.AddSingleton<WorkspaceAvailabilityPersistenceMapper>();
            services.AddSingleton<WorkspacePersistenceMapper>();
            services.AddSingleton<WorkspaceCalendarPersistenceMapper>();
            services.AddSingleton<WorkspaceCalendarBookingPersistenceMapper>();

            return services;
        }
    }
}