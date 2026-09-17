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

            foreach (var publish in publishers)
            {
                services.AddSingleton(typeof(IPublish), publish);
            }

            services.AddSingleton<PublishConnectionService>();

            return services;
        }
    }
}