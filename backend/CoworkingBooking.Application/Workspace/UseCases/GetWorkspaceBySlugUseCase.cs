using CoworkingBooking.Application.Interfaces;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Shared.Classes;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Application.Workspace.UseCases
{
    public class GetWorkspaceBySlugUseCase: IUseCase<string, DetailsWorkspaceResponseDTO>
    {
        private readonly IWorkspaceRepository workspaceRepository;
        private readonly WorkspaceMapper workspaceMapper;
        private readonly ILogger<GetWorkspaceBySlugUseCase> logger;

        public GetWorkspaceBySlugUseCase(
            IWorkspaceRepository workspaceRepository, 
            WorkspaceMapper workspaceMapper,
            ILogger<GetWorkspaceBySlugUseCase> logger
        )
        {
            this.workspaceRepository = workspaceRepository;
            this.workspaceMapper = workspaceMapper;
            this.logger = logger;
        }

        public async Task<Result<DetailsWorkspaceResponseDTO>> Execute(string slug)
        {
            try
            {
                logger.LogInformation("Searching Workspace by Slug: {Slug}", slug);

                var workspaceFound = await workspaceRepository.FindOneBySlug(slug);

                if (workspaceFound == null)
                {
                    logger.LogWarning("Workspace with Slug {Slug} not found", slug);
                    return Result<DetailsWorkspaceResponseDTO>.Failure(new List<Error> { new Error("Workspace not found.", ErrorType.NotFound) });
                }

                var response = workspaceMapper.ToDetailsResponseDTO(workspaceFound);

                logger.LogInformation("Workspace by {Slug} found", slug);
                return Result<DetailsWorkspaceResponseDTO>.Success(response);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException) 
                {
                    logger.LogWarning(ex, ex.Message);
                    return Result<DetailsWorkspaceResponseDTO>.Failure(new List<Error> { new Error(ex.Message, ErrorType.ValidationError) });
                }

                logger.LogError(ex, "Occured unexpected error.");
                return Result<DetailsWorkspaceResponseDTO>.Failure(new List<Error> { new Error("An unexpected error occurred.", ErrorType.InternalServerError) });
            }
        }
    }
}