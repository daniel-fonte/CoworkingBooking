using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Infraestructure.Models;
using CoworkingBooking.Shared.Utils;

namespace CoworkingBooking.Infraestructure.Mappers
{
    public class WorkspacePersistenceMapper
    {
        private readonly WorkspaceAvailabilityPersistenceMapper workspaceAvailabilityPersistenceMapper;

        public WorkspacePersistenceMapper(
            WorkspaceAvailabilityPersistenceMapper workspaceAvailabilityPersistenceMapper
        ) {
            this.workspaceAvailabilityPersistenceMapper = workspaceAvailabilityPersistenceMapper;
        }

        public WorkspaceModel ToModel(WorkspaceEntity entity)
        {
            var coordinates = ConvertGeoJsonObjectModelToGeoJson.ParseToGeoJsonMongoDB(entity.Coordinates);

            return new WorkspaceModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug,
                Description = entity.Description,
                Status = entity.Status,
                IsInactive = entity.IsInactive,
                PricePerHour = entity.PricePerHour,
                Resources = entity.Resources.ToList(),
                Type = entity.Type,
                Availability = entity.Availability is null ? null : workspaceAvailabilityPersistenceMapper.ToModel(entity.Availability),
                Coordinates = coordinates,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public WorkspaceEntity ToEntity(WorkspaceModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var coordinates = ConvertGeoJsonObjectModelToGeoJson.ParseToGeoJson(model.Coordinates);

            return WorkspaceEntity.Rehydrate(
                id: model.Id,
                name: model.Name,
                description: model.Description,
                slug: model.Slug,
                type: model.Type,
                status: model.Status,
                coordinates: coordinates,
                pricePerHour: model.PricePerHour,
                isInactive: model.IsInactive,
                resources: model.Resources,
                createdAt: model.CreatedAt,
                updatedAt: model.UpdatedAt,
                availability: model.Availability is null ? null : workspaceAvailabilityPersistenceMapper.ToEntity(model.Availability)
            );
        }
    }
}