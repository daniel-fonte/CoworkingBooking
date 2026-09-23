using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Infraestructure.Models;

namespace CoworkingBooking.Infraestructure.Mappers
{
    public class WorkspaceCalendarPersistenceMapper
    {
        private readonly WorkspaceCalendarBookingPersistenceMapper workspaceCalendarBookingPersistenceMapper;

        public WorkspaceCalendarPersistenceMapper(WorkspaceCalendarBookingPersistenceMapper workspaceCalendarBookingPersistenceMapper)
        {
            this.workspaceCalendarBookingPersistenceMapper = workspaceCalendarBookingPersistenceMapper;
        }

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

        public WorkspaceCalendarEntity ToEntity(WorkspaceCalendarModel model)
        {
            return WorkspaceCalendarEntity.Rehydrate(
                id: model.Id,
                workspaceId: model.WorkspaceId,
                startAt: model.StartAt,
                endAt: model.EndAt,
                isFull: model.IsFull,
                isInactive: model.IsInactive,
                bookings: model.Bookings?.Select(workspaceCalendarBookingPersistenceMapper.ToEntity).ToList() ?? [],
                createdAt: model.CreatedAt,
                updatedAt: model.UpdatedAt
            );
        }
    }
}