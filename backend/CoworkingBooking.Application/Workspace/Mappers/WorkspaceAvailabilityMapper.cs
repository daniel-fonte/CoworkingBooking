using System.Globalization;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Core.Workspace.Entities;
using Microsoft.VisualBasic;

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
                StartAt: workspaceAvailability.StartAt.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture),
                EndAt: workspaceAvailability.EndAt.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture),
                Frequency: workspaceAvailability.Recurrence.Frequency,
                Until: workspaceAvailability.Recurrence.Until.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture),
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

        private int DayOfWeekEnum(DayOfWeek dayOfWeek)
        {
            return (int)dayOfWeek;
        }

    }
}