using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Shared.Classes;
using System.Globalization;
using CoworkingBooking.Application.Workspace.Cache;

namespace CoworkingBooking.Application.Workspace.Mappers
{
    public class WorkspaceMapper
    {
        private readonly WorkspaceAvailabilityMapper workspaceAvailabilityMapper;

        public WorkspaceMapper(
            WorkspaceAvailabilityMapper workspaceAvailabilityMapper
        )
        {
            this.workspaceAvailabilityMapper = workspaceAvailabilityMapper;
        }

        public WorkspaceEntity ToEntity(CreateWorkspaceRequestDTO workspaceDTO)
        {
            var coordinates = GeoJsonMapper(workspaceDTO.Coordinates);

            return new WorkspaceEntity(
                workspaceDTO.Name, 
                workspaceDTO.Description, 
                workspaceDTO.Type, 
                coordinates, 
                workspaceDTO.PricePerHour,
                workspaceDTO.Resources
            );
        }
        
        public WorkspaceEntity ToEntity(WorkspaceCache workspaceCache)
        {
            var workspaceAvailability = workspaceAvailabilityMapper.ToEntity(workspaceCache.Availability);

            return WorkspaceEntity.Rehydrate(
                workspaceCache.Id,
                workspaceCache.Name,
                workspaceCache.Description,
                workspaceCache.Slug,
                workspaceCache.Type,
                workspaceCache.Status,
                GeoJsonMapper(workspaceCache.Coordinates),
                workspaceCache.PricePerHour,
                workspaceCache.IsInactive,
                workspaceCache.Resources,
                workspaceCache.CreatedAt,
                workspaceCache.UpdatedAt,
                workspaceAvailability
            );
        }

        public CreateWorkspaceResponseDTO ToCreateResponseDTO(WorkspaceEntity workspaceEntity)
        {
            return new CreateWorkspaceResponseDTO(
                workspaceEntity.Slug,
                workspaceEntity.Name,
                workspaceEntity.Description,
                workspaceEntity.Status,
                workspaceEntity.Type,
                GeoJsonToCoordinates(workspaceEntity.Coordinates),
                workspaceEntity.PricePerHour,
                workspaceEntity.Resources.ToList()
            );
        }

        public DetailsWorkspaceResponseDTO ToDetailsResponseDTO(WorkspaceEntity workspaceEntity)
        {
            return new DetailsWorkspaceResponseDTO(
                workspaceEntity.Name,
                workspaceEntity.Description,
                workspaceEntity.Slug,
                workspaceEntity.Status,
                workspaceEntity.Type,
                GeoJsonToCoordinates(workspaceEntity.Coordinates),
                workspaceEntity.PricePerHour,
                workspaceEntity.IsInactive,
                workspaceEntity.Resources.ToList(),
                workspaceEntity.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture),
                workspaceEntity.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture)
            );
        }

        public UpdateWorkspaceStatusResponseDTO ToWorkspaceStatusResponseDTO(WorkspaceEntity workspaceEntity)
        {
            return new UpdateWorkspaceStatusResponseDTO(
                workspaceEntity.Name,
                workspaceEntity.Description,
                workspaceEntity.Slug,
                workspaceEntity.Status,
                workspaceEntity.Type,
                GeoJsonToCoordinates(workspaceEntity.Coordinates),
                workspaceEntity.PricePerHour,
                workspaceEntity.IsInactive,
                workspaceEntity.Resources.ToList(),
                workspaceEntity.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture),
                workspaceEntity.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture)
            );
        }

        private GeoJson GeoJsonMapper(Coordinates coordinates)
        {
            if (coordinates == null)
            {
                throw new ArgumentNullException(nameof(coordinates), "Coordinates must be oject.");
            }

            return new GeoJson
            {
                type = "Point",
                coordinates = new double[] { coordinates.lgn, coordinates.lat }
            };
        }

        private Coordinates GeoJsonToCoordinates(GeoJson geoJson)
        {
            if (geoJson == null || geoJson.coordinates == null || geoJson.coordinates.Length != 2)
            {
                throw new ArgumentException("GeoJson must have coordinates array of two elements.", nameof(geoJson));
            }

            double longitude = geoJson.coordinates[0];
            double latitude = geoJson.coordinates[1];

            return new Coordinates { lat = latitude, lgn = longitude};
        }

        private GeoJson GeoJsonMapper(WorkspaceCoordinatesCache workspaceCoordinatesCache)
        {
            return new GeoJson
            {
                type = workspaceCoordinatesCache.Type,
                coordinates = workspaceCoordinatesCache.Coordinates
            };
        }
    }
}