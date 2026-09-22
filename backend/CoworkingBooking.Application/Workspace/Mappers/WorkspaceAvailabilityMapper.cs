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

        public UpdateWorkspaceAvailabilityResponseDTO ToWorkspaceAvailabilityResponseDTO(WorkSpaceAvailability workspaceAvailability)
        {
            return new UpdateWorkspaceAvailabilityResponseDTO(
                StartAt: DatesUtils.ToISOString(workspaceAvailability.StartAt),
                EndAt: DatesUtils.ToISOString(workspaceAvailability.EndAt),
                Frequency: workspaceAvailability.Recurrence.Frequency,
                Until: DatesUtils.ToISOString(workspaceAvailability.Recurrence.Until),
                ByDay: workspaceAvailability.Recurrence.ByDay,
                ByMonth: workspaceAvailability.Recurrence.ByMonth,
                Timezone: workspaceAvailability.Timezone
            );
        }

        // public List<UpdateWorkspaceAvailabilityResponseDTO> ToWorkspaceAvailabilityResponseDTOList(List<WorkSpaceAvailability> workspaceEntities)
        // {
        //     return workspaceEntities.Select(entity => ToWorkspaceAvailabilityResponseDTO(entity)).ToList();
        // }

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
    }
}