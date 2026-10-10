using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CoworkingBooking.Api.DependencyInjection
{
    public static class MongoConfigurationsService
    {
        public static void AddMongoConfigurationsService()
        {
            BsonSerializer.RegisterSerializer(
                new EnumSerializer<DayOfWeek>(BsonType.String)
            );

            BsonSerializer.RegisterSerializer(
                new GuidSerializer(GuidRepresentation.Standard)
            );
        }
    }
}