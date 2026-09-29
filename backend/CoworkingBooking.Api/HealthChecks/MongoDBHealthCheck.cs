using System.Diagnostics;
using CoworkingBooking.Infraestructure.Providers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;

namespace CoworkingBooking.Api.HealthChecks
{
    public sealed class MongoDBHealthCheck : IHealthCheck
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        
        public MongoDBHealthCheck(
            MongodbDatabaseService mongodbDatabaseService
        )
        {
            _mongodbDatabaseService = mongodbDatabaseService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var startAt = Stopwatch.GetTimestamp();

                await _mongodbDatabaseService.GetMongoClient().GetDatabase("admin")
                    .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1),  cancellationToken: cancellationToken);


                var elapsed = Stopwatch.GetElapsedTime(startAt);

                return elapsed.TotalMilliseconds > 5000
                    ? HealthCheckResult.Degraded($"MongoDB slow: {elapsed.TotalMilliseconds:F0} ms")
                    : HealthCheckResult.Healthy($"MongoDB responded in {elapsed.TotalMilliseconds:F0} ms");
            }
            catch (System.Exception ex)
            {
                return HealthCheckResult.Unhealthy("MongoDB unexpected error", ex);
            }
        }
    }
}