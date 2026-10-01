using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.SQS;
using CoworkingBooking.Api.Json;
using CoworkingBooking.Api.Providers;
using CoworkingBooking.Application;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Serilog;

namespace CoworkingBooking.Api.DependencyInjection
{
    public static class EssentialsConfigurationsService
    {
        public static IServiceCollection AddEssentialsConfigurationsService(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            
            BsonSerializer.RegisterSerializer(
                new EnumSerializer<DayOfWeek>(BsonType.String)
            );

            services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty(
                    "Application",
                    configuration["Application:Name"]
                )
                .WriteTo.Seq(configuration.GetSection("Seq")["ServerUrl"] ?? "http://localhost:5342"));

            services
                .AddControllers(options => {
                    options.ModelBinderProviders.Insert(0, new ObjectModelBinderProvider());
                    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                })
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.Converters.Add(new WorkspaceTypeJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            services.AddValidatorsFromAssemblyContaining<ApplicationAssembly>();

            services.AddHostedService<RedisMonitorService>();

            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSQS>();

            return services;
        }
    }
}