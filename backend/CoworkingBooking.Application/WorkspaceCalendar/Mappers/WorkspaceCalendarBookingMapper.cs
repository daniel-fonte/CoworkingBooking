using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Core.WorkspaceCalendar.Entities;

namespace CoworkingBooking.Application.WorkspaceCalendar.Mappers
{
    public class WorkspaceCalendarBookingMapper
    {
        public WorkspaceCalendarBooking ToEntity(CreateWorkspaceCalendarBookingRequestDTO dto)
        {
            return new WorkspaceCalendarBooking(DateTime.Parse(dto.StartAt), DateTime.Parse(dto.EndAt));
        }
    }
}