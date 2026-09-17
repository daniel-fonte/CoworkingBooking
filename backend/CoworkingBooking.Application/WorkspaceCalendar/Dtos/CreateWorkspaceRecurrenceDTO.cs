using CoworkingBooking.Core.Workspace.Entities;

namespace CoworkingBooking.Application.WorkspaceCalendar.Dtos
{
    public sealed record CreateWorkspaceRecurrenceRequestDTO(
        string WorkspaceId,
        WorkSpaceAvailability WorkSpaceAvailability
    );
}