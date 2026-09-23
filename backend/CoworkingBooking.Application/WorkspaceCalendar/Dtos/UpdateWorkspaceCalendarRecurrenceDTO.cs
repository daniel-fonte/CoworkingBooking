using CoworkingBooking.Core.Workspace.Entities;

namespace CoworkingBooking.Application.WorkspaceCalendar.Dtos
{
    public sealed record UpdateWorkspaceCalendarRecurrenceRequestDTO(
        string WorkspaceId,
        WorkSpaceAvailability WorkSpaceAvailability
    );
}