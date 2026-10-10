using CoworkingBooking.Api.Classes;
using CoworkingBooking.Api.Providers;
using CoworkingBooking.Application;
using CoworkingBooking.Application.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CoworkingBooking.Api.DependencyInjection
{
    public static class AuthenticationServices
    {
        public static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddAuthentication(opts =>
                {
                    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opts =>
                {
                    opts.Authority = configuration["Keycloak:Authority"];
                    opts.Audience = configuration["Keycloak:Audience"];
                    opts.RequireHttpsMetadata = false;
                });

            services.AddAuthorization();

            services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

            services.AddSingleton<ICurrentUserService, CurrentUserSerivce>();

            return services;
        }
    }
}