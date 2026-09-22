using CoworkingBooking.Application.WorkspaceCalendar.Ports;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Repositories;
namespace CoworkingBooking.Application.Workspace.Adapters
{
    public class WorkspaceExistenceCheckerAdapter : IWorkspaceExistenceCheckerPort
    {
        private readonly IWorkspaceRepository workspaceRepository;

        public WorkspaceExistenceCheckerAdapter(IWorkspaceRepository workspaceRepository)
        {
            this.workspaceRepository = workspaceRepository;
        }

        public async Task<WorkspaceEntity?> Execute(string workspaceId)
        {
            return await workspaceRepository.FindOneById(workspaceId);
        }
    }
}