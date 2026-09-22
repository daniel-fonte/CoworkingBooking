using CoworkingBooking.Application.WorkspaceCalendar.Ports;
using CoworkingBooking.Core.Workspace.Repositories;

namespace CoworkingBooking.Application.Workspace.Adapters
{
    public class GetWorkspaceAvailabilityTimezoneAdapter : IGetWorkspaceAvailabilityTimezonePort
    {
        private readonly IWorkspaceRepository workspaceRepository;

        public GetWorkspaceAvailabilityTimezoneAdapter(
            IWorkspaceRepository workspaceRepository
        )
        {
            this.workspaceRepository = workspaceRepository;
        }

        public async Task<string?> Execute(string workspaceId)
        {
            var workspaceTimezoneFound = await workspaceRepository.FindTimezoneById(workspaceId);

            return workspaceTimezoneFound;
        }
    }
}