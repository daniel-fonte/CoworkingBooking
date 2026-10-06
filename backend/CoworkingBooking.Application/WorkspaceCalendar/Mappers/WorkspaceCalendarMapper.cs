using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Shared.Utils;

namespace CoworkingBooking.Application.WorkspaceCalendar.Mappers
{
    public class WorkspaceCalendarMapper
    {
        public GetWorkspaceCalendarRecurrenceResponseDTO ToGetWorkspaceCalendarRecurrenceResponseDTO(WorkspaceCalendarEntity entity)
        {
            return new GetWorkspaceCalendarRecurrenceResponseDTO(
                DatesUtils.ToISOString(entity.StartAt),
                DatesUtils.ToISOString(entity.EndAt),
                entity.IsFull
            );
        }
    }
}