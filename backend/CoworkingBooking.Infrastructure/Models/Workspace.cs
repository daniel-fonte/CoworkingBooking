using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace CoworkingBooking.Infraestructure.Models
{
    [BsonIgnoreExtraElements]
    public class WorkspaceModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        [BsonRepresentation(BsonType.String)]
        public string Description { get; set; } = string.Empty;

        [BsonElement("slug")]
        [BsonRepresentation(BsonType.String)]
        public string Slug { get; set; } = string.Empty;

        [BsonElement("type")]
        [BsonRepresentation(BsonType.String)]
        public WorkspaceType Type { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public WorkspaceStatus Status { get; set; }

        [BsonElement("coordinates")]
        public required GeoJsonPoint<GeoJson2DGeographicCoordinates> Coordinates { get; set; }

        [BsonElement("pricePerHour")]
        [BsonRepresentation(BsonType.Double)]
        public double PricePerHour { get; set; }

        [BsonElement("isInactive")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsInactive { get; set; }

        [BsonElement("resources")]
        public List<string> Resources { get; set; } = new List<string>();

        [BsonElement("createdAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("availability")]
        [BsonIgnoreIfNull]
        public WorkSpaceAvailabilityModel? Availability { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class WorkSpaceAvailabilityModel
    {
        [BsonElement("startAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime StartAt { get; set; }

        [BsonElement("endAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime EndAt { get; set; }

        [BsonElement("recurrence")]
        public WorkSpaceAvailabilityRecurrenceModel Recurrence { get; set; } = new WorkSpaceAvailabilityRecurrenceModel();

        [BsonElement("timezone")]
        public string Timezone { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class WorkSpaceAvailabilityRecurrenceModel
    {
        [BsonElement("frequency")]
        [BsonRepresentation(BsonType.String)]
        public Frequency Frequency { get; set; }

        [BsonElement("interval")]
        [BsonRepresentation(BsonType.Int32)]
        public int Interval { get; set; }

        [BsonElement("until")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Until { get; set; }

        [BsonElement("byDay")]
        [BsonIgnoreIfNull]
        public List<DayOfWeek>? ByDay { get; set; }

        [BsonElement("byMonth")]
        [BsonIgnoreIfNull]
        public List<int>? ByMonth { get; set; }
    }
}