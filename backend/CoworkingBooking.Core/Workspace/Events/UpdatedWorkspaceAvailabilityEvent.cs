using CoworkingBooking.Core.Workspace.Entities;

namespace CoworkingBooking.Core.Workspace.Events
{
    public sealed record UpdatedWorkspaceAvailabilityEvent(
        string WorkspaceId,
        WorkSpaceAvailability WorkSpaceAvailability
    );
}