using CoworkingBooking.Infraestructure.Providers;
using CoworkingBooking.Api.Json;
using CoworkingBooking.Api.Providers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Scalar.AspNetCore;
using FluentValidation;
using CoworkingBooking.Application;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Amazon.SQS;
using CoworkingBooking.Api.Exceptions;
using CoworkingBooking.Api.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up the application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
    
    BsonSerializer.RegisterSerializer(
        new EnumSerializer<DayOfWeek>(BsonType.String)
    );

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty(
            "Application",
            builder.Configuration["Application:Name"]
        )
        .WriteTo.Seq(builder.Configuration.GetSection("Seq")["ServerUrl"] ?? "http://localhost:5342"));

    builder.Services
        .AddControllers(options => {
            options.ModelBinderProviders.Insert(0, new ObjectModelBinderProvider());
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        })
        .AddJsonOptions(options => {
            options.JsonSerializerOptions.Converters.Add(new WorkspaceTypeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

    builder.Services.AddHealthChecks()
        .AddCheck<RedisHealthCheck>("redis", tags: ["ready"])
        .AddCheck<MongoDBHealthCheck>("mongoDB", tags: ["ready"]);

    builder.Services.AddOpenApi(options => {
        options.AddSchemaTransformer(new WorkspaceOpenApiSchemaTransformer());
        options.AddOperationTransformer(new WorkspaceOpenApiOperationTransformer());
    });

    builder.Services.AddExceptionHandler<RedisExceptionHandler>();

    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
    builder.Services.AddAWSService<IAmazonSQS>();

    builder.Services.AddApplicationServices();

    builder.Services.AddHostedService<RedisMonitorService>();

    builder.Services.AddValidatorsFromAssemblyContaining<ApplicationAssembly>();

    var app = builder.Build();

    app.Services.GetRequiredService<MongodbDatabaseService>();

    using (var scope = app.Services.CreateScope())
    {
        var redisService = scope.ServiceProvider
            .GetRequiredService<RedisService>();

        try
        {
            await redisService.ValidateConnectionAsync(5, TimeSpan.FromSeconds(3));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, ex.Message);
            throw;
        }
    }

    // Executa as migrations
    using (var scope = app.Services.CreateScope())
    {
        var migrationRunner = scope.ServiceProvider
            .GetRequiredService<MigrationRunner>();

        await migrationRunner.RunMigrations();
    }

    // Executa a QueueUrlConnection
    using (var scope = app.Services.CreateScope())
    {
        var migrationRunner = scope.ServiceProvider
            .GetRequiredService<PublishConnectionService>(); 

        await migrationRunner.GetQueuesUrl();
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference("/docs");
    }

    app.UseExceptionHandler(_ => { });
    app.UseStatusCodePages();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.MapControllers();

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                totalDurationMs = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    durationMs = entry.Value.Duration.TotalMilliseconds,
                    error = entry.Value.Exception?.Message
                })
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
       Predicate = _ => false,
       ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                totalDurationMs = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    durationMs = entry.Value.Duration.TotalMilliseconds,
                    error = entry.Value.Exception?.Message
                })
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    });

    app.Run();
}
catch (System.Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

