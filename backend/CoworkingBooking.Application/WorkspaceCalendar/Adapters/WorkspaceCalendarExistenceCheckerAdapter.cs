using CoworkingBooking.Application.Workspace.Ports;
using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;

namespace CoworkingBooking.Application.WorkspaceCalendar.Adapters
{
    public class WorkspaceCalendarExistenceCheckerAdapter : IWorkspaceCalendarExistenceCheckerPort
    {
        private readonly IWorkspaceCalendarRepository _workspaceCalendarRepository;

        public WorkspaceCalendarExistenceCheckerAdapter(
            IWorkspaceCalendarRepository workspaceCalendarRepository
        )
        {
            _workspaceCalendarRepository = workspaceCalendarRepository;
        }

        public async Task<List<WorkspaceCalendarEntity>> Execute(string workspaceId, DateTime startAt, DateTime endAt)
        {
            return await _workspaceCalendarRepository.FindByWorkspaceAvailability(workspaceId, startAt, endAt);
        }
    }
}