using CoworkingBooking.Core.Workspace.Entities;

namespace CoworkingBooking.Application.WorkspaceCalendar.Dtos
{
    public sealed record CreateWorkspaceCalendarRecurrenceRequestDTO(
        string WorkspaceId,
        WorkSpaceAvailability WorkSpaceAvailability
    );
}