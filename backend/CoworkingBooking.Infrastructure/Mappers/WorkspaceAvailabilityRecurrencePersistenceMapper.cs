using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Infraestructure.Models;

namespace CoworkingBooking.Infraestructure.Mappers
{
    public class WorkspaceAvailabilityRecurrencePersistenceMapper
    {
        public WorkSpaceAvailabilityRecurrenceModel ToModel(WorkSpaceAvailabilityRecurrence recurrence)
        {
            return new WorkSpaceAvailabilityRecurrenceModel()
            {
                Frequency = recurrence.Frequency,
                Until = recurrence.Until,
                Interval = recurrence.Interval,
                ByDay = recurrence.ByDay,
                ByMonth = recurrence.ByMonth
            };
        }

        public WorkSpaceAvailabilityRecurrence ToEntity(WorkSpaceAvailabilityRecurrenceModel model)
        {
            return new WorkSpaceAvailabilityRecurrence(
                frequency: model.Frequency,
                until: model.Until,
                byDay: model.ByDay,
                byMonth: model.ByMonth
            );
        }
    }
}