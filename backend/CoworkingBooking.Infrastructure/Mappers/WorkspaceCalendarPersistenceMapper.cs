using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Infraestructure.Models;

namespace CoworkingBooking.Infraestructure.Mappers
{
    public class WorkspaceCalendarPersistenceMapper
    {
        public WorkspaceCalendarModel ToModel(WorkspaceCalendarEntity entity)
        {
            return new WorkspaceCalendarModel
            {
                Id = entity.Id,
                WorkspaceId = entity.WorkspaceId,
                StartAt = entity.StartAt,
                EndAt = entity.EndAt,
                IsFull = entity.IsFull,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}