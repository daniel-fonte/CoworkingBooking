using System.Reflection;
using CoworkingBooking.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CoworkingBooking.Infraestructure.DependencyInjection
{
    public static class RepositoryService
    {
        public static IServiceCollection AddRepositoriesService(
            this IServiceCollection services,
            Assembly assembly
        )
        {
            var openType = typeof(IRepository<>);

            var repositories = assembly.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == openType));

            foreach (var repository in repositories)
            {
                services.AddSingleton(repository);

                foreach (var @interface in repository.GetInterfaces())
                {
                    services.AddSingleton(
                        @interface,
                        sp => sp.GetRequiredService(repository)
                    );
                }
            }

            return services;
        }
    }
}