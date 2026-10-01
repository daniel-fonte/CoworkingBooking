using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Shared.Utils;

namespace CoworkingBooking.Application.Workspace.Mappers
{
    public class WorkspaceAvailabilityMapper
    {
        private readonly WorkspaceAvailabilityRecurrenceMapper workspaceAvailabilityRecurrenceMapper;
        
        public WorkspaceAvailabilityMapper(
            WorkspaceAvailabilityRecurrenceMapper workspaceAvailabilityRecurrenceMapper
        )
        {
            this.workspaceAvailabilityRecurrenceMapper = workspaceAvailabilityRecurrenceMapper;
        }

        public WorkSpaceAvailability ToEntity(UpdateWorkspaceAvailabilityRequestDTO availability)
        {
            return new WorkSpaceAvailability(
                DateTime.Parse(availability.StartAt),
                DateTime.Parse(availability.EndAt),
                recurrence: workspaceAvailabilityRecurrenceMapper.ToEntity(
                    frequency: availability.Frequency,
                    until: availability.Until,
                    byDay: availability.ByDay,
                    byMonth: availability.ByMonth
                ),
                availability.Timezone
            );
        }

        public UpdateWorkspaceAvailabilityResponseDTO ToWorkspaceAvailabilityResponseDTO(WorkSpaceAvailability workSpaceAvailability)
        {
            return new UpdateWorkspaceAvailabilityResponseDTO(
                StartAt: DatesUtils.ToISOString(workSpaceAvailability.StartAt),
                EndAt: DatesUtils.ToISOString(workSpaceAvailability.EndAt),
                Frequency: workSpaceAvailability.Recurrence.Frequency,
                Until: DatesUtils.ToISOString(workSpaceAvailability.Recurrence.Until),
                ByDay: workSpaceAvailability.Recurrence.ByDay,
                ByMonth: workSpaceAvailability.Recurrence.ByMonth,
                Timezone: workSpaceAvailability.Timezone
            );
        }
    }
}