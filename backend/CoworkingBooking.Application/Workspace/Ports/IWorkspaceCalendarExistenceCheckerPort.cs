using CoworkingBooking.Core.WorkspaceCalendar.Entities;

namespace CoworkingBooking.Application.Workspace.Ports
{
    public interface IWorkspaceCalendarExistenceCheckerPort
    {
        Task<List<WorkspaceCalendarEntity>> Execute(string workspaceId, DateTime startAt, DateTime endAt);
    }
}