using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Shared.Utils;

namespace CoworkingBooking.Application.WorkspaceCalendar.Mappers
{
    public class WorkspaceCalendarBookingMapper
    {
        public WorkspaceCalendarBooking ToEntity(CreateWorkspaceCalendarBookingRequestDTO dto)
        {
            return new WorkspaceCalendarBooking(DateTime.Parse(dto.StartAt), DateTime.Parse(dto.EndAt));
        }

        public CreateWorkspaceCalendarBookingResponseDTO ToCreateResponseDTO(WorkspaceCalendarBooking workspaceEntity)
        {
            return new CreateWorkspaceCalendarBookingResponseDTO(
                DatesUtils.ToISOString(workspaceEntity.StartAt),
                DatesUtils.ToISOString(workspaceEntity.EndAt),
                workspaceEntity.TotalPrice
            );
        }
    }
}