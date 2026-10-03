using CoworkingBooking.Core.WorkspaceCalendar.Repositories;

namespace CoworkingBooking.Infraestructure.CronJobs
{
    public sealed class CleanWorkspaceCalendarRecurrencesCronJob
    {
        private readonly IWorkspaceCalendarRepository workspaceCalendarRepository;

        public CleanWorkspaceCalendarRecurrencesCronJob(IWorkspaceCalendarRepository workspaceCalendarRepository)
        {
            this.workspaceCalendarRepository = workspaceCalendarRepository;
        }

        public async Task ExecuteAsync()
        {
            var count = await workspaceCalendarRepository
                .DeleteMany(x => x.IsInactive, true);
        }
    }
}