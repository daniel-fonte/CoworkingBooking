using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Infraestructure.Models;

namespace CoworkingBooking.Infraestructure.Mappers
{
    public class WorkspaceCalendarBookingPersistenceMapper
    {
        public WorkspaceCalendarBookingModel ToModel(WorkspaceCalendarBooking entity)
        {
            return new WorkspaceCalendarBookingModel
            {
                StartAt = entity.StartAt,
                EndAt = entity.EndAt,
                TotalPrice = entity.TotalPrice,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public WorkspaceCalendarBooking ToEntity(WorkspaceCalendarBookingModel model)
        {
            return WorkspaceCalendarBooking.Rehydrate(
                startAt: model.StartAt,
                endAt: model.EndAt,
                model.UserId,
                totalPrice: model.TotalPrice,
                createdAt: model.CreatedAt,
                updatedAt: model.UpdatedAt
            );
        }
    }
}