using CoworkingBooking.Core.Workspace.Entities;

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
    }
}