using System.Linq.Expressions;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Core.WorkspaceCalendar.Entities;

namespace CoworkingBooking.Core.Workspace.Repositories
{
    public interface IWorkspaceRepository
    {
        Task<WorkspaceEntity?> FindOneById(string id);
        Task<WorkspaceEntity?> FindOneBySlug(string slug);
        Task<List<WorkspaceEntity>> FindAll();
        Task<WorkspaceEntity> InsertOne(WorkspaceEntity workspace);
        Task<WorkspaceEntity?> UpdateAvailability(string workspaceId, WorkSpaceAvailability availability);
        Task<WorkspaceEntity> UpdateOneById(string workspaceId, WorkspaceEntity workspace);
        Task<WorkspaceEntity> UpdateStatusById(string id, WorkspaceStatus status);
    }
}