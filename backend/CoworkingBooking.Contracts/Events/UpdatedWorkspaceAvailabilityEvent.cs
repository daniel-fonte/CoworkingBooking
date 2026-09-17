namespace CoworkingBooking.Contracts.Events
{
    public sealed record UpdatedWorkspaceAvailabilityEvent(
        string WorkspaceId,
        WorkspaceAvailabilityContract WorkSpaceAvailability
    );
}