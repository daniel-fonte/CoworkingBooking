using CoworkingBooking.Api.Classes;
using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Application.WorkspaceCalendar.UseCases;
using CoworkingBooking.Shared.Classes;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingBooking.Api.Controllers
{
    [ApiController]
    [Route("api/workspaceCalendar")]
    public class WorkspaceCalendarController : ControllerBase
    {
        private readonly CreateWorkspaceCalendarBookingUseCase createWorkspaceCalendarBookingUsaCase;

        public WorkspaceCalendarController(
            CreateWorkspaceCalendarBookingUseCase createWorkspaceCalendarBookingUsaCase
        )
        {
            this.createWorkspaceCalendarBookingUsaCase = createWorkspaceCalendarBookingUsaCase;
        }

        [HttpPost("{calendarId}/booking")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<CreateWorkspaceCalendarBookingResponseDTO>>> CreateBooking(
            [FromRoute] string calendarId,
            [FromBody] CreateWorkspaceCalendarBookingRequestDTO body
        )
        {
            var result = await createWorkspaceCalendarBookingUsaCase.Execute((calendarId, body));

            return ResultsExtension.ToActionResult(result, data => CreatedAtAction(nameof(CreateBooking), new { calendarId = calendarId }, data));
        }
    }
}