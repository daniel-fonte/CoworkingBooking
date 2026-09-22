namespace CoworkingBooking.Application.WorkspaceCalendar.Ports
{
    public interface IGetWorkspaceAvailabilityTimezonePort
    {
        Task<string?> Execute(string workspaceId);
    }
}