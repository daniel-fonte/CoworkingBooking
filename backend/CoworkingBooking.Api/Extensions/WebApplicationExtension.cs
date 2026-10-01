using Scalar.AspNetCore;
using Serilog;

namespace CoworkingBooking.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseApplicationPipeline(
            this WebApplication app)
        {
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

            app.MapApplicationHealthChecks();

            return app;
        }
    }
}