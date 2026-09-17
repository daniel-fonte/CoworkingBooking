using CoworkingBooking.Infraestructure.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CoworkingBooking.Infraestructure.Providers
{
    public static class MigrationServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoMigrations(
            this IServiceCollection services,
            Assembly assembly
        ) {
            var migrationType = typeof(AbstractMigration);

            var migrations = assembly.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    migrationType.IsAssignableFrom(t));

            foreach (var migration in migrations)
            {
                services.AddSingleton(typeof(AbstractMigration), migration);
            }

            services.AddSingleton<MigrationRunner>();

            return services;
        }
    }
}