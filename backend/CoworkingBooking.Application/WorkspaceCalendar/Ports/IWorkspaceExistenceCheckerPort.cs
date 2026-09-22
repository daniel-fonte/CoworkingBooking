using CoworkingBooking.Core.Workspace.Entities;

namespace CoworkingBooking.Application.WorkspaceCalendar.Ports
{
    public interface IWorkspaceExistenceCheckerPort
    {
        Task<WorkspaceEntity?> Execute(string workspaceId);
    }
}