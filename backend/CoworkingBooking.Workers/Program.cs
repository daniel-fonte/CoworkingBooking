using Amazon.SQS;
using CoworkingBooking.Application.WorkspaceCalendar.UseCases;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Infraestructure;
using CoworkingBooking.Infraestructure.Mappers;
using CoworkingBooking.Infraestructure.Providers;
using CoworkingBooking.Shared.Interfaces;
using CoworkingBooking.Workers;
using CoworkingBooking.Workers.Consumers;
using CoworkingBooking.Workers.Mappers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty(
        "Application",
        builder.Configuration["Application:Name"]
    )
    .WriteTo.Seq(builder.Configuration.GetSection("Seq")["ServerUrl"] ?? "http://localhost:5342"));

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

builder.Services.AddSingleton<MongodbDatabaseService>();

builder.Services.AddSingleton<WorkspaceAvailabilityMapper>();
builder.Services.AddSingleton<WorkspaceCalendarPersistenceMapper>();
builder.Services.AddSingleton<WorkspaceCalendarMapper>();
builder.Services.AddSingleton<WorkspaceCalendarBookingPersistenceMapper>();

builder.Services.AddSingleton<IWorkspaceCalendarRepository, WorkspaceCalendarRepository>();
builder.Services.AddScoped<ITransactionManager, MongodbTransactionManagerService>();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSQS>();

builder.Services.AddScoped<UpdatedWorkspaceAvailabilityConsumer>();
builder.Services.AddScoped<UpdateWorkspaceCalendarRecurrencesUseCase>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Services.GetRequiredService<MongodbDatabaseService>();

host.Run();
