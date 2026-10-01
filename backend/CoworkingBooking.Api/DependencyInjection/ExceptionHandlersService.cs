using System.Reflection;
using CoworkingBooking.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace CoworkingBooking.Api.DependencyInjection
{
    public static class ExceptionHandlersService
    {
        public static IServiceCollection AddExceptionsHandlersServices(
            this IServiceCollection services
        )
        {
            services.AddExceptionHandler<RedisExceptionHandler>();

            return services;
        }
    }
}