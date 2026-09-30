using System.Reflection;
using CoworkingBooking.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CoworkingBooking.Infraestructure.Providers
{
    public static class PublishServices
    {
        public static IServiceCollection AddPublishers(
            this IServiceCollection services,
            Assembly assembly
        ) {
            var publishType = typeof(IPublish);

            var publishers = assembly.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    publishType.IsAssignableFrom(t));

            foreach (var publisher in publishers)
            {
                services.AddSingleton(publisher);

                var interfaces = publisher
                    .GetInterfaces()
                    .Where(i => i != typeof(IPublish));

                foreach (var @interface in interfaces)
                {
                    services.AddSingleton(
                        @interface,
                        sp => sp.GetRequiredService(publisher)
                    );
                }

                services.AddSingleton(
                    publishType,
                    sp => sp.GetRequiredService(publisher)
                );
            }

            services.AddSingleton<PublishConnectionService>();

            return services;
        }
    }
}
