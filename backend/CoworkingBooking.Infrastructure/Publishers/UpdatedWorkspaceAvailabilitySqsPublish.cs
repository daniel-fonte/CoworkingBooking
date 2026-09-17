using CoworkingBooking.Shared.Classes;
using Amazon.SQS;
using Microsoft.Extensions.Logging;
using Amazon.SQS.Model;
using System.Text.Json;
using CoworkingBooking.Shared.Interfaces;
using CoworkingBooking.Shared.Enums;

namespace CoworkingBooking.Infraestructure.Publishers
{
    public class UpdatedWorkspaceAvailabilityPublish : IPublish
    {
        private readonly IAmazonSQS sqsClient;
        private readonly ILogger<UpdatedWorkspaceAvailabilityPublish> logger;
        private readonly string UpdatedWorkspaceAvailabilityQueueName = Queues.WorkspaceAvailabilityUpdate;
        private string? QueueUrl;

        public UpdatedWorkspaceAvailabilityPublish(
            IAmazonSQS sqsClient,
            ILogger<UpdatedWorkspaceAvailabilityPublish> logger
        )
        {
            this.sqsClient = sqsClient;
            this.logger = logger;
        }

        public async Task Initialize()
        {

            logger.LogInformation("Getting QueueUrl to Queue: {Name}", UpdatedWorkspaceAvailabilityQueueName);
            var request = new GetQueueUrlRequest
            {
                QueueName = Queues.WorkspaceAvailabilityUpdate
            };

            var queueUrlResponse = await sqsClient.GetQueueUrlAsync(request);

            QueueUrl = queueUrlResponse.QueueUrl;
        }

        public async Task<Result<bool>> EnqueueMessage<UpdatedWorkspaceAvailabilityEvent>(UpdatedWorkspaceAvailabilityEvent message)
        {
            if (QueueUrl is null) throw new InvalidOperationException("Queue URL was not initialized");

            var sendMessageRequest = new SendMessageRequest()
            {
                QueueUrl = QueueUrl,
                MessageBody = JsonSerializer.Serialize(message)
            };

            logger.LogInformation("Publishing message to Queue {queueName} with body : \n {request}", UpdatedWorkspaceAvailabilityQueueName, sendMessageRequest.MessageBody);
            
            await sqsClient.SendMessageAsync(sendMessageRequest);
            
           return Result<bool>.Success(true);
        }
    }
}