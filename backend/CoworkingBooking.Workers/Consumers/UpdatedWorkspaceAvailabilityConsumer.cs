using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using CoworkingBooking.Application.WorkspaceCalendar.UseCases;
using CoworkingBooking.Contracts.Events;
using CoworkingBooking.Workers.Mappers;

namespace CoworkingBooking.Workers.Consumers
{
    public class UpdatedWorkspaceAvailabilityConsumer
    {
        private readonly IAmazonSQS sqsClient;
        private readonly ILogger<UpdatedWorkspaceAvailabilityConsumer> logger;
        private readonly string UpdatedWorkspaceAvailabilityQueueName = "workspace-availability";
        private readonly CreateWorkspaceRecurrencesUseCase createWorkspaceRecurrencesUseCase;
        private readonly WorkspaceCalendarMapper workspaceCalendarMapper;
        private string? queueUrl;

        public UpdatedWorkspaceAvailabilityConsumer(
            IAmazonSQS sqsClient,
            ILogger<UpdatedWorkspaceAvailabilityConsumer> logger,
            CreateWorkspaceRecurrencesUseCase createWorkspaceRecurrencesUseCase,
            WorkspaceCalendarMapper workspaceCalendarMapper
        ) {
            this.sqsClient = sqsClient;
            this.logger = logger;
            this.createWorkspaceRecurrencesUseCase = createWorkspaceRecurrencesUseCase;
            this.workspaceCalendarMapper = workspaceCalendarMapper;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            var request = new GetQueueUrlRequest
            {
                QueueName = UpdatedWorkspaceAvailabilityQueueName
            };

            logger.LogInformation("Getting Queue URL to queue: {QueueName}", UpdatedWorkspaceAvailabilityQueueName);

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
                        var data = JsonSerializer.Deserialize<UpdatedWorkspaceAvailabilityEvent>(message.Body);

                        if (data is null)
                        {
                            this.logger.LogWarning("Message body is invalid: {Message}", message.Body);
                            throw new ArgumentNullException("Message body is invalid");
                        }

                        var workspaceCalendarToCreate = this.workspaceCalendarMapper.ToCreateWorkspaceRecurrenceRequestDTO(data);

                        var result = await createWorkspaceRecurrencesUseCase.Execute(workspaceCalendarToCreate);

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
                            logger.LogWarning("Message deserialize to {DTO} failed", typeof(UpdatedWorkspaceAvailabilityEvent));
                        }

                        throw;
                    }
                }
                
            }
        }
    }
}