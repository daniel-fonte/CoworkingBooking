using CoworkingBooking.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Infraestructure.Providers
{
    public class PublishConnectionService
    {
        private readonly ILogger<PublishConnectionService> _logger;
        private readonly IEnumerable<IPublish> _publishers;

        public PublishConnectionService(
            ILogger<PublishConnectionService> logger, 
            IEnumerable<IPublish> publishers)
        {
            _publishers = publishers;
            _logger = logger;
        }

        public async Task GetQueuesUrl()
        {
            _logger.LogInformation("Starting Getting QueueUrl");

            foreach (var publish in _publishers)
            {
                await publish.Initialize();
            }
        }
    }
}