using CoworkingBooking.Application.Interfaces;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Shared.Classes;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Application.Workspace.UseCases
{
    public class UpdateWorkspaceStatusUseCase : IUseCase<(string slug, UpdateWorkspaceStatusRequestDTO request), UpdateWorkspaceStatusResponseDTO>
    {
        private readonly ILogger<UpdateWorkspaceStatusUseCase> logger;
        private readonly IValidator<UpdateWorkspaceStatusRequestDTO> validator;
        private readonly IWorkspaceRepository repository;
        private readonly WorkspaceMapper mapper;

        public UpdateWorkspaceStatusUseCase(
            ILogger<UpdateWorkspaceStatusUseCase> logger,
            IValidator<UpdateWorkspaceStatusRequestDTO> validator,
            IWorkspaceRepository repository,
            WorkspaceMapper mapper
        )
        {
            this.logger = logger;
            this.validator = validator;
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<UpdateWorkspaceStatusResponseDTO>> Execute((string slug, UpdateWorkspaceStatusRequestDTO request) input)
        {
            try
            {
                var (slug, request) = input;

                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return Result<UpdateWorkspaceStatusResponseDTO>.Failure(
                        validationResult.Errors.Select(e => new Error(e.ErrorMessage, ErrorType.ValidationError)).ToList()
                    );
                }

                var workspaceFound = await repository.FindOneBySlug(slug);

                if (workspaceFound == null)
                {
                    logger.LogWarning("Workspace with Slug {Slug} not found", slug);
                    return Result<UpdateWorkspaceStatusResponseDTO>.Failure(new List<Error> { new Error("Workspace not found.", ErrorType.NotFound) });
                }

                logger.LogInformation("Initied update status {Status} to Workspace {Slug}", slug, request.status);

                workspaceFound.UpdateStatus(request.status);

                var updateResult = await repository.UpdateOneById(workspaceFound.Id, workspaceFound);

                if (updateResult == null)
                {
                    logger.LogWarning("Failed to update Workspace {Slug} status", slug);
                    return Result<UpdateWorkspaceStatusResponseDTO>.Failure(new List<Error> { new Error("Failed to update workspace availability.", ErrorType.InternalServerError) });
                }

                logger.LogInformation("Workspace {Slug} status updated sucessfully", workspaceFound.Slug);

                var response = mapper.ToWorkspaceStatusResponseDTO(updateResult);

                return Result<UpdateWorkspaceStatusResponseDTO>.Success(response);

            }
            catch (System.Exception ex)
            {
                if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException) 
                {
                    logger.LogWarning(ex, ex.Message);
                    return Result<UpdateWorkspaceStatusResponseDTO>.Failure(new List<Error> { new Error(ex.Message, ErrorType.ValidationError) });
                }

                logger.LogError(ex, "Occured unexpected error.");
                return Result<UpdateWorkspaceStatusResponseDTO>.Failure(new List<Error> { new Error("An unexpected error occurred.", ErrorType.InternalServerError) });
            }
        }
    }
}