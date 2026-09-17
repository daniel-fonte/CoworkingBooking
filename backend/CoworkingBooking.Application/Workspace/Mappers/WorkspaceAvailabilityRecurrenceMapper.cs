using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Shared.Enums;

namespace CoworkingBooking.Application.Workspace.Mappers
{
    public class WorkspaceAvailabilityRecurrenceMapper
    {
        public WorkSpaceAvailabilityRecurrence ToEntity(Frequency frequency, string until, List<DayOfWeek>? byDay, List<int>? byMonth)
        {
            return new WorkSpaceAvailabilityRecurrence(
                frequency,
                DateTime.Parse(until),
                byDay,
                byMonth
            );
        }
    }
}