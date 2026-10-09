using CoworkingBooking.Api.Providers;
using Serilog;
using CoworkingBooking.Api.DependencyInjection;
using CoworkingBooking.Infraestructure.DependencyInjection;
using CoworkingBooking.Api.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up the application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddEssentialsConfigurationsService(builder.Configuration);

    builder.Services.AddDocumentationService();

    builder.Services.AddHealthCheckServices(builder.Configuration);

    builder.Services.AddExceptionsHandlersServices();

    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddHangfireServices(builder.Configuration);

    builder.Services.AddApplicationServices();

    builder.Services.AddAuthenticationServices(builder.Configuration);

    var app = builder.Build();

    await app.InitializeApplicationAsync();

    app.UseApplicationPipeline();

    HangfireExtensions.AddRecurringJobs();

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

