using CoworkingBooking.Api.Classes;
using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Application.WorkspaceCalendar.UseCases;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Enums;
using CoworkingBooking.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingBooking.Api.Controllers
{
    [ApiController]
    [Route("api/workspaceCalendar")]
    public class WorkspaceCalendarController : ControllerBase
    {
        private readonly CreateWorkspaceCalendarBookingUseCase createWorkspaceCalendarBookingUsaCase;
        private readonly GetWorkspaceCalendarRecurrencesUseCase getWorkspaceCalendarRecurrencesUseCase;

        public WorkspaceCalendarController(
            CreateWorkspaceCalendarBookingUseCase createWorkspaceCalendarBookingUsaCase,
            GetWorkspaceCalendarRecurrencesUseCase getWorkspaceCalendarRecurrencesUseCase
        )
        {
            this.createWorkspaceCalendarBookingUsaCase = createWorkspaceCalendarBookingUsaCase;
            this.getWorkspaceCalendarRecurrencesUseCase = getWorkspaceCalendarRecurrencesUseCase;
        }

        [HttpPost("{calendarId}/booking")]
        [Authorize(Roles = Roles.Admin)]
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

        [HttpGet("{workspaceId}")]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.User}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<CursorPaginationRecordResponse<GetWorkspaceCalendarRecurrenceResponseDTO>>>> GetWorkspaceCalendarRecurrences(
            [FromRoute] string workspaceId,
            [FromQuery] CursorPaginationRecordRequest cursorPaginationRecordRequest,
            CancellationToken cancellationToken
        )
        {
            var result = await getWorkspaceCalendarRecurrencesUseCase.Execute((workspaceId, cursorPaginationRecordRequest, cancellationToken));

            return ResultsExtension.ToActionResult(result, data => Ok(data));
        }
    }
}