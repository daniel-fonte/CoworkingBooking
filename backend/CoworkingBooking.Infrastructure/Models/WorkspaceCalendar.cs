using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CoworkingBooking.Infraestructure.Models
{
    [BsonIgnoreExtraElements]
    public class WorkspaceCalendarModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("workspaceId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string WorkspaceId { get; set; } = string.Empty;

        [BsonElement("starAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime StartAt { get; set; }

        [BsonElement("endAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime EndAt { get; set; }

        [BsonElement("isFull")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsFull { get; set; } = false;

        [BsonElement("bookings")]
        [BsonIgnoreIfNull]
        public List<WorkspaceCalendarBookingModel>? Bookings { get; set; }

        [BsonElement("createdAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

    }

    [BsonIgnoreExtraElements]
    public class WorkspaceCalendarBookingModel
    {
        [BsonElement("starAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime StartAt { get; set; }

        [BsonElement("endAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime EndAt { get; set; }

        [BsonElement("totalPrice")]
        [BsonRepresentation(BsonType.Double)]
        public double TotalPrice { get; set; }

        [BsonElement("createdAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }
    }
}