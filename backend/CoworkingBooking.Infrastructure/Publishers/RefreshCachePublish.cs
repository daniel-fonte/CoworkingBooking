using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Enums;
using CoworkingBooking.Shared.Publishers;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Infraestructure.Publishers
{
    public class RefreshCachePublish : IRefreshCachePublisher
    {
        private readonly IAmazonSQS sqsClient;
        private readonly ILogger<RefreshCachePublish> logger;
        private readonly string QueueName = Queues.RefreshCache;
        private string? QueueUrl;

        public RefreshCachePublish(
            IAmazonSQS sqsClient,
            ILogger<RefreshCachePublish> logger
        )
        {
            this.sqsClient = sqsClient;
            this.logger = logger;
        }

        public async Task<Result<bool>> EnqueueMessage<T>(T @event)
        {
            if (QueueUrl is null) throw new InvalidOperationException("Queue URL was not initialized");

            var sendMessageRequest = new SendMessageRequest()
            {
                QueueUrl = QueueUrl,
                MessageBody = JsonSerializer.Serialize(@event)
            };

            logger.LogInformation("Publishing message to Queue {queueName} with body : \n {request}", QueueName, sendMessageRequest.MessageBody);
            
            await sqsClient.SendMessageAsync(sendMessageRequest);
            
            return Result<bool>.Success(true);
        }

        public async Task Initialize()
        {
            logger.LogInformation("Getting QueueUrl to Queue: {Name}", QueueName);
            
            var request = new GetQueueUrlRequest
            {
                QueueName = QueueName
            };

            var queueUrlResponse = await sqsClient.GetQueueUrlAsync(request);

            QueueUrl = queueUrlResponse.QueueUrl;
        }
    }
}