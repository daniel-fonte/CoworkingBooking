using System.Reflection;
using CoworkingBooking.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CoworkingBooking.Api.Providers
{
    public static class UseCasesServiceExtensions
    {
        public static IServiceCollection AddUseCases(
            this IServiceCollection services,
            Assembly assembly
        ) {
            var useCaseTypes = typeof(IUseCase<,>);

            var types = assembly.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == useCaseTypes));

            foreach (var useCaseType in types)
            {
                services.AddTransient(useCaseType);
            }

            return services;
        }
    }
}