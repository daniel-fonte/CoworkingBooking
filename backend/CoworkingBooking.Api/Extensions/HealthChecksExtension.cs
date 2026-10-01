using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CoworkingBooking.Api.Extensions
{
    public static class HealthChecksExtension
    {
        public static WebApplication MapApplicationHealthChecks(
            this WebApplication app)
        {
            app.MapHealthChecks(
                "/health/ready",
                new HealthCheckOptions
                {
                    Predicate = check =>
                        check.Tags.Contains("ready"),

                    ResponseWriter = WriteResponse
                });

            app.MapHealthChecks(
                "/health/live",
                new HealthCheckOptions
                {
                    Predicate = _ => false,

                    ResponseWriter = WriteResponse
                });

            return app;
        }

        private static async Task WriteResponse(
            HttpContext context,
            HealthReport report
        )
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
    }
}