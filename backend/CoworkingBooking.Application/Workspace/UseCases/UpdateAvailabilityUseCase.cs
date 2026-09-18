using CoworkingBooking.Application.Interfaces;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Core.Workspace.Events;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Interfaces;
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

        public UpdateAvailabilityUseCase(
            IWorkspaceRepository workspaceRepository,
            IValidator<UpdateWorkspaceAvailabilityRequestDTO> validator,
            WorkspaceMapper workspaceMapper,
            WorkspaceAvailabilityMapper workspaceAvailabilityMapper,
            WorkspaceAvailabilityRecurrenceMapper workspaceAvailabilityRecurrenceMapper,
            ILogger<UpdateAvailabilityUseCase> logger,
            IPublish updatedWorkspaceAvailabilityPublish
        ) {
            this.workspaceRepository = workspaceRepository;
            this.validator = validator;
            this.workspaceMapper = workspaceMapper;
            this.workspaceAvailabilityRecurrenceMapper = workspaceAvailabilityRecurrenceMapper;
            this.workspaceAvailabilityMapper = workspaceAvailabilityMapper;
            this.logger = logger;
            this.updatedWorkspaceAvailabilityPublish = updatedWorkspaceAvailabilityPublish;
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

                // // Update the availability of the workspace
                workspaceFound.UpdateAvailability(workspaceAvailability);
                
                var updateResult = await workspaceRepository.UpdateAvailability(workspaceFound.Id, workspaceFound.Availability!);

                if (updateResult == null)
                {
                    return Result<UpdateWorkspaceAvailabilityResponseDTO>
                        .Failure(new List<Error> { new Error("Failed to update workspace availability.", ErrorType.InternalServerError) });
                }

                var response = this.workspaceAvailabilityMapper.ToWorkspaceAvailabilityResponseDTO(updateResult.Availability!);

                logger.LogInformation("Workspace {Slug} availability updated sucessfully", slug);

                await this.updatedWorkspaceAvailabilityPublish.EnqueueMessage<UpdatedWorkspaceAvailabilityEvent>(
                    new UpdatedWorkspaceAvailabilityEvent(workspaceFound.Id, updateResult.Availability!)
                );

                return Result<UpdateWorkspaceAvailabilityResponseDTO>.Success(response);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException) 
                {
                    logger.LogWarning(ex, ex.Message);
                    return Result<UpdateWorkspaceAvailabilityResponseDTO>.Failure(new List<Error> { new Error(ex.Message, ErrorType.ValidationError) });
                }

                logger.LogError(ex, "Occured unexpected error.");
                return Result<UpdateWorkspaceAvailabilityResponseDTO>.Failure(new List<Error> { new Error("An unexpected error occurred.", ErrorType.InternalServerError) });
            }
        }
    }
}