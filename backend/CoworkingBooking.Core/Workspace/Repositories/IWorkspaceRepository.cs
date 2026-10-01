using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Shared.Interfaces;

namespace CoworkingBooking.Core.Workspace.Repositories
{
    public interface IWorkspaceRepository : IRepository<WorkspaceEntity>
    {
        Task<WorkspaceEntity?> FindOneBySlug(string slug);
        Task<string?> FindTimezoneById(string id);
        Task<WorkspaceEntity?> UpdateAvailability(string workspaceId, WorkSpaceAvailability availability);
        Task<WorkspaceEntity> UpdateStatusById(string id, WorkspaceStatus status);
    }
}