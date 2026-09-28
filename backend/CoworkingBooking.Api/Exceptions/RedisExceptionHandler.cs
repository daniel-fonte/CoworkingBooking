using CoworkingBooking.Shared.Classes;
using Microsoft.AspNetCore.Diagnostics;
using StackExchange.Redis;

namespace CoworkingBooking.Api.Exceptions
{
    public class RedisExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<RedisExceptionHandler> logger;

        public RedisExceptionHandler(
            ILogger<RedisExceptionHandler> logger
        )
        {
            this.logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            if (exception is not (RedisConnectionException or RedisTimeoutException)) return false;

            httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            httpContext.Response.Headers.RetryAfter = "10";
            httpContext.Response.ContentType = "application/json";

            var respose = new ApiResponse(false, [ new Error("Redis temporary unvaible", ErrorType.InternalServerError) ]);

            logger.LogDebug("Redis temporary unvaible");

            await httpContext.Response.WriteAsJsonAsync(
                respose,
                cancellationToken
            );

            return true;
            
        }
    }
}