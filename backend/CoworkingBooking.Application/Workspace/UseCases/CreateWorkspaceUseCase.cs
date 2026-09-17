using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Shared.Exceptions;
using FluentValidation;
using Slugify;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Application.Workspace.UseCases
{
    public class CreateWorkspaceUseCase : IUseCase<CreateWorkspaceRequestDTO, CreateWorkspaceResponseDTO>
    {
        private readonly IWorkspaceRepository workspaceRepository;
        private readonly IValidator<CreateWorkspaceRequestDTO> validator;
        private readonly WorkspaceMapper workspaceMapper;
        private readonly ILogger<CreateWorkspaceUseCase> logger;

        public CreateWorkspaceUseCase(
            ILogger<CreateWorkspaceUseCase> logger,
            IWorkspaceRepository workspaceRepository, 
            IValidator<CreateWorkspaceRequestDTO> validator, 
            WorkspaceMapper workspaceMapper
        ) {
            this.workspaceRepository = workspaceRepository;
            this.validator = validator;
            this.workspaceMapper = workspaceMapper;
            this.logger = logger;
        }

        public async Task<Result<CreateWorkspaceResponseDTO>> Execute(CreateWorkspaceRequestDTO workspace)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(workspace);

                if (!validationResult.IsValid)
                {
                    return Result<CreateWorkspaceResponseDTO>.Failure(
                        validationResult.Errors.Select(e => new Error(e.ErrorMessage, ErrorType.ValidationError)).ToList()
                    );
                }

                logger.LogInformation("Initiated creating to Workspace: {Name} ", workspace.Name);

                var workspaceEntity = workspaceMapper.ToEntity(workspace);

                SlugHelper helper = new SlugHelper();

                workspaceEntity.Publish(helper.GenerateSlug(workspaceEntity.Name));

                var result = await workspaceRepository.InsertOne(workspaceEntity);

                logger.LogInformation("Workspace: {Id} - {Name} saved on database", result.Id, workspaceEntity.Name);

                var response = workspaceMapper.ToCreateResponseDTO(result);

                return Result<CreateWorkspaceResponseDTO>.Success(response);
            }
            catch (Exception ex)
            {
                if(ex is DuplicateKeyException)
                {
                    logger.LogWarning(ex, ex.Message);
                    return Result<CreateWorkspaceResponseDTO>.Failure(new List<Error> { new Error("A workspace with the same name already exists.", ErrorType.Conflict) });
                } 
                else if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException) 
                {
                    logger.LogWarning(ex, ex.Message);
                    return Result<CreateWorkspaceResponseDTO>.Failure(new List<Error> { new Error(ex.Message, ErrorType.ValidationError) });
                }

                logger.LogError(ex, "Occured unexpected error.");
                return Result<CreateWorkspaceResponseDTO>.Failure(new List<Error> { new Error(ex.Message, ErrorType.InternalServerError) });
            }
        }
    }
}