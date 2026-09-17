using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CoworkingBooking.Infraestructure.Models
{
    [BsonIgnoreExtraElements]
    public class MigrationModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
    }
}