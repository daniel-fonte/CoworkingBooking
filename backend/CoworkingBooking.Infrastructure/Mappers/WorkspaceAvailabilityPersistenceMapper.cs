using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Infraestructure.Models;

namespace CoworkingBooking.Infraestructure.Mappers
{
    public class WorkspaceAvailabilityPersistenceMapper
    {

        private readonly WorkspaceAvailabilityRecurrencePersistenceMapper workspaceAvailabilityRecurrencePersistenceMapper;

        public WorkspaceAvailabilityPersistenceMapper(
            WorkspaceAvailabilityRecurrencePersistenceMapper workspaceAvailabilityRecurrencePersistenceMapper
        )
        {
            this.workspaceAvailabilityRecurrencePersistenceMapper = workspaceAvailabilityRecurrencePersistenceMapper;
        }

        public WorkSpaceAvailabilityModel ToModel(WorkSpaceAvailability entity)
        {
            return new WorkSpaceAvailabilityModel
            {
                StartAt = entity.StartAt,
                EndAt = entity.EndAt,
                Recurrence = workspaceAvailabilityRecurrencePersistenceMapper.ToModel(entity.Recurrence),
                Timezone = entity.Timezone
            };
        }

        public WorkSpaceAvailability ToEntity(WorkSpaceAvailabilityModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            return WorkSpaceAvailability.Rehydrate(
                startAt: model.StartAt,
                endAt: model.EndAt,
                recurrence: workspaceAvailabilityRecurrencePersistenceMapper.ToEntity(model.Recurrence),
                timezone: model.Timezone
            );
        }
    }

}