using CoworkingBooking.Api.Classes;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Application.Workspace.UseCases;
using CoworkingBooking.Shared.Classes;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingBooking.Api.Controllers
{
    [ApiController]
    [Route("api/workspace")]
    public class WorkspaceController : ControllerBase
    {
        private readonly CreateWorkspaceUseCase createWorkspaceUseCase;
        private readonly GetWorkspaceBySlugUseCase getWorkspaceBySlugUseCase;
        private readonly UpdateAvailabilityUseCase updateAvailabilityUseCase;
        private readonly UpdateWorkspaceStatusUseCase updateWorkspaceStatusUseCase;

        public WorkspaceController(
            CreateWorkspaceUseCase createWorkspaceUseCase,
            GetWorkspaceBySlugUseCase getWorkspaceBySlugUseCase,
            UpdateAvailabilityUseCase updateAvailabilityUseCase,
            UpdateWorkspaceStatusUseCase updateWorkspaceStatusUseCase
        ) {
            this.createWorkspaceUseCase = createWorkspaceUseCase;
            this.getWorkspaceBySlugUseCase = getWorkspaceBySlugUseCase;
            this.updateAvailabilityUseCase = updateAvailabilityUseCase;
            this.updateWorkspaceStatusUseCase = updateWorkspaceStatusUseCase;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<CreateWorkspaceResponseDTO>>> CreateWorkspace(
            [FromBody] CreateWorkspaceRequestDTO request
        ) {
            var result = await createWorkspaceUseCase.Execute(request);

            return ResultsExtension.ToActionResult(result, data => CreatedAtAction(nameof(GetBySlug), new { slug = data.Data!.Slug }, data));
        }

        [HttpGet("{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<DetailsWorkspaceResponseDTO>>> GetBySlug(
            [FromRoute] string slug
        ) {
            var result = await getWorkspaceBySlugUseCase.Execute(slug);

            return ResultsExtension.ToActionResult(result, data => Ok(data));
        }
    
        [HttpPatch("{slug}/availability")]
        public async Task<ActionResult<ApiResponse<UpdateWorkspaceAvailabilityResponseDTO>>> UpdateWorkspaceAvailability(
            [FromRoute] string slug,
            [FromBody] UpdateWorkspaceAvailabilityRequestDTO request
        ) {
            var result = await updateAvailabilityUseCase.Execute((slug, request));

            return ResultsExtension.ToActionResult(result, data => Ok(data));
        }

        [HttpPatch("{slug}/status")]
        public async Task<ActionResult<ApiResponse<UpdateWorkspaceStatusResponseDTO>>> UpdateWorkspaceStatus(
            [FromRoute] string slug,
            [FromBody] UpdateWorkspaceStatusRequestDTO request
        ) {
            var result = await updateWorkspaceStatusUseCase.Execute((slug, request));

            return ResultsExtension.ToActionResult(result, data => Ok(data));
        }
    }
}