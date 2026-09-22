using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Contracts.Events;

namespace CoworkingBooking.Workers.Mappers
{
    public class WorkspaceCalendarMapper
    {
        private readonly WorkspaceAvailabilityMapper mapper;

        public WorkspaceCalendarMapper(WorkspaceAvailabilityMapper mapper)
        {
            this.mapper = mapper;
        }

        public CreateWorkspaceCalendarRecurrenceRequestDTO ToCreateWorkspaceRecurrenceRequestDTO(UpdatedWorkspaceAvailabilityEvent @event)
        {
            return new CreateWorkspaceCalendarRecurrenceRequestDTO(
                WorkspaceId: @event.WorkspaceId,
                WorkSpaceAvailability: mapper.ToEntity(@event.WorkSpaceAvailability)
            );
        }
    }
}