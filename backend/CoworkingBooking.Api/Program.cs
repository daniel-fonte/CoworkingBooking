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

    builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
    builder.Services.AddAWSService<IAmazonSQS>();
    
    BsonSerializer.RegisterSerializer(
        new EnumSerializer<DayOfWeek>(BsonType.String)
    );

    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddApplicationServices();

    builder.Services.AddValidatorsFromAssemblyContaining<ApplicationAssembly>();

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

    builder.Services.AddOpenApi(options => {
        options.AddSchemaTransformer(new WorkspaceOpenApiSchemaTransformer());
        options.AddOperationTransformer(new WorkspaceOpenApiOperationTransformer());
    });

    var app = builder.Build();

    app.Services.GetRequiredService<MongodbDatabaseService>();

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

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.MapControllers(); 

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

