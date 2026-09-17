using CoworkingBooking.Contracts.Events;
using CoworkingBooking.Core.Workspace.Entities;

namespace CoworkingBooking.Workers.Mappers
{
    public class WorkspaceAvailabilityMapper
    {
        public WorkSpaceAvailability ToEntity(WorkspaceAvailabilityContract workspaceAvailabilityContract)
        {
            return new WorkSpaceAvailability(
                startAt: workspaceAvailabilityContract.StartAt,
                endAt: workspaceAvailabilityContract.EndAt,
                timezone: workspaceAvailabilityContract.Timezone,
                recurrence: new WorkSpaceAvailabilityRecurrence(
                    frequency: workspaceAvailabilityContract.Recurrence.Frequency,
                    until: workspaceAvailabilityContract.Recurrence.Until,
                    byDay: workspaceAvailabilityContract.Recurrence.ByDay,
                    byMonth: workspaceAvailabilityContract.Recurrence.ByMonth
                )
            );
        }
    }
}