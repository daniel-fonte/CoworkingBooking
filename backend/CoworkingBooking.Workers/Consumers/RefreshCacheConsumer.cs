using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using CoworkingBooking.Application.Workspace;
using CoworkingBooking.Shared.Events;
using CoworkingBooking.Shared.Interfaces;

namespace CoworkingBooking.Workers.Consumers
{
    public class RefreshCacheConsumer
    {
        private readonly IAmazonSQS sqsClient;
        private readonly ILogger<RefreshCacheConsumer> logger;
        private readonly string QueueName = "refresh-cache";
        private readonly IServiceProvider serviceProvider;
        private readonly WorkspaceRefreshCacheHandler workspaceRefreshCacheHandler;
        private string? queueUrl;

        public RefreshCacheConsumer(
            IAmazonSQS sqsClient,
            ILogger<RefreshCacheConsumer> logger,
            IServiceProvider serviceProvider,
            WorkspaceRefreshCacheHandler workspaceRefreshCacheHandler
        ) {
            this.sqsClient = sqsClient;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.workspaceRefreshCacheHandler = workspaceRefreshCacheHandler;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            var request = new GetQueueUrlRequest
            {
                QueueName = QueueName
            };

            logger.LogInformation("Getting Queue URL to queue: {QueueName}", QueueName);

            GetQueueUrlResponse queueUrlResponse = await sqsClient.GetQueueUrlAsync(request, cancellationToken);

            this.queueUrl = queueUrlResponse.QueueUrl;
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken)
        {
            if (queueUrl is null) throw new InvalidOperationException("Queue URL was not initialized");
            
            var receiveMessageRequest = new ReceiveMessageRequest()
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 20
            };

            var messageResponse = await this.sqsClient.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);

            if (messageResponse?.Messages?.Count > 0)
            {
                foreach (var message in messageResponse?.Messages ?? [])
                {
                    if (string.IsNullOrEmpty(message.Body))
                    {
                        logger.LogWarning("Message receveid is empty: {Message}", message.Body);
                    }

                    logger.LogInformation("Processing Message: {Message} | {Time}", message.Body, DateTimeOffset.Now);

                    try
                    {
                        var data = JsonSerializer.Deserialize<RefreshCacheEvent>(message.Body);

                        if (data is null)
                        {
                            logger.LogWarning("Message body is invalid: {Message}", message.Body);
                            throw new ArgumentNullException("Message body is invalid");
                        }

                        // var serviceType = typeof(IRefreshCacheService<>)
                        //     .MakeGenericType(cacheType);

                        // dynamic service = serviceProvider.GetRequiredService(serviceType);

                        await workspaceRefreshCacheHandler.Execute(data.cacheKey, cancellationToken);

                        var deleteMessageRequest = new DeleteMessageRequest
                        {
                            QueueUrl = queueUrl,
                            ReceiptHandle = message.ReceiptHandle,
                        };

                        await sqsClient.DeleteMessageAsync(deleteMessageRequest, cancellationToken);
                        
                    }
                    catch (System.Exception ex)
                    {
                        if (ex is JsonException)
                        {
                            logger.LogWarning("Message deserialize to {DTO} failed", typeof(RefreshCacheEvent));
                        }

                        throw;
                    }
                }
                
            }
        }
    }
}