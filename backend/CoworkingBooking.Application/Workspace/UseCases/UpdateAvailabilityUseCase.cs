using CoworkingBooking.Application.Interfaces;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Application.Workspace.Ports;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Core.Workspace.Events;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Interfaces;
using CoworkingBooking.Shared.Utils;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Application.Workspace.UseCases
{
    public class UpdateAvailabilityUseCase : IUseCase<(string slug, UpdateWorkspaceAvailabilityRequestDTO availability), UpdateWorkspaceAvailabilityResponseDTO>
    {
        private readonly IWorkspaceRepository workspaceRepository;
        private readonly IValidator<UpdateWorkspaceAvailabilityRequestDTO> validator;
        private readonly WorkspaceMapper workspaceMapper;
        private readonly WorkspaceAvailabilityMapper workspaceAvailabilityMapper;
        private readonly WorkspaceAvailabilityRecurrenceMapper workspaceAvailabilityRecurrenceMapper;
        private readonly ILogger<UpdateAvailabilityUseCase> logger;
        private readonly IPublish updatedWorkspaceAvailabilityPublish;
        private readonly IWorkspaceCalendarExistenceCheckerPort workspaceCalendarExistenceChecker;

        public UpdateAvailabilityUseCase(
            IWorkspaceRepository workspaceRepository,
            IValidator<UpdateWorkspaceAvailabilityRequestDTO> validator,
            WorkspaceMapper workspaceMapper,
            WorkspaceAvailabilityMapper workspaceAvailabilityMapper,
            WorkspaceAvailabilityRecurrenceMapper workspaceAvailabilityRecurrenceMapper,
            ILogger<UpdateAvailabilityUseCase> logger,
            IPublish updatedWorkspaceAvailabilityPublish,
            IWorkspaceCalendarExistenceCheckerPort workspaceCalendarExistenceChecker
        ) {
            this.workspaceRepository = workspaceRepository;
            this.validator = validator;
            this.workspaceMapper = workspaceMapper;
            this.workspaceAvailabilityRecurrenceMapper = workspaceAvailabilityRecurrenceMapper;
            this.workspaceAvailabilityMapper = workspaceAvailabilityMapper;
            this.logger = logger;
            this.updatedWorkspaceAvailabilityPublish = updatedWorkspaceAvailabilityPublish;
            this.workspaceCalendarExistenceChecker = workspaceCalendarExistenceChecker;
        }

        public async Task<Result<UpdateWorkspaceAvailabilityResponseDTO>> Execute((string slug, UpdateWorkspaceAvailabilityRequestDTO availability) input)
        {
            try
            {
                var (slug, availability) = input;
                
                var validationResult = await validator.ValidateAsync(availability);

                if (!validationResult.IsValid)
                {
                    return Result<UpdateWorkspaceAvailabilityResponseDTO>.Failure(
                        validationResult.Errors.Select(e => new Error(e.ErrorMessage, ErrorType.ValidationError)).ToList()
                    );
                }

                logger.LogInformation("Initied Workspace availability update to Workspace {Slug}", slug);

                var workspaceAvailability = workspaceAvailabilityMapper.ToEntity(availability);
              
                var workspaceFound = await workspaceRepository.FindOneBySlug(slug);

                if (workspaceFound == null)
                {
                    return Result<UpdateWorkspaceAvailabilityResponseDTO>
                        .Failure(new List<Error> { new Error("Workspace not found.", ErrorType.NotFound) });
                }

                var workspaceCalendarExistence = await workspaceCalendarExistenceChecker
                    .Execute(workspaceFound.Id, workspaceAvailability.StartAt, workspaceAvailability.Recurrence.Until);

                if (
                    workspaceCalendarExistence.Count > 0 &&  
                    (workspaceCalendarExistence.First().StartAt == DateTime.Parse(availability.StartAt)) &&
                    (workspaceCalendarExistence.Last().EndAt == DateTime.Parse(availability.Until))
                )
                {
                    logger.LogWarning("Already exists Workspace Availability to {StartAt} - {Until}", availability.StartAt, availability.Until);
                    return Result<UpdateWorkspaceAvailabilityResponseDTO>
                        .Failure([new Error($"Already exists Workspace Availability to {availability.StartAt} - {availability.Until}", ErrorType.ValidationError)]);
                }

                workspaceFound.UpdateAvailability(workspaceAvailability, workspaceCalendarExistence);

                var updatedAvailabilityResult = await workspaceRepository.UpdateAvailability(workspaceFound.Id, workspaceFound.Availability!);

                if (updatedAvailabilityResult == null)
                {
                    return Result<UpdateWorkspaceAvailabilityResponseDTO>
                        .Failure([new Error("Failed to update workspace availability.", ErrorType.InternalServerError)]);
                }

                workspaceFound.UpdateStatus(WorkspaceStatus.Available);

                var updatedWorkspaceStatus = await workspaceRepository.UpdateStatusById(workspaceFound.Id, workspaceFound.Status);

                var response = this.workspaceAvailabilityMapper.ToWorkspaceAvailabilityResponseDTO(updatedWorkspaceStatus.Availability!);

                logger.LogInformation("Workspace {Slug} availability updated sucessfully", slug);

                await this.updatedWorkspaceAvailabilityPublish.EnqueueMessage<UpdatedWorkspaceAvailabilityEvent>(
                    new UpdatedWorkspaceAvailabilityEvent(workspaceFound.Id, updatedWorkspaceStatus.Availability!)
                );

                return Result<UpdateWorkspaceAvailabilityResponseDTO>.Success(response);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException) 
                {
                    logger.LogWarning(ex, ex.Message);
                    return Result<UpdateWorkspaceAvailabilityResponseDTO>
                        .Failure(new List<Error> { new Error(ex.Message, ErrorType.ValidationError) });
                }

                logger.LogError(ex, "Occured unexpected error.");
                return Result<UpdateWorkspaceAvailabilityResponseDTO>
                    .Failure(new List<Error> { new Error("An unexpected error occurred.", ErrorType.InternalServerError) });
            }
        }
    }
}