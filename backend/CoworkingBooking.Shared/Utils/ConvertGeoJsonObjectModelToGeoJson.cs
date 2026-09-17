using MongoDB.Driver.GeoJsonObjectModel;
using CoworkingBooking.Shared.Classes;

namespace CoworkingBooking.Shared.Utils
{
    public class ConvertGeoJsonObjectModelToGeoJson
    {
        public static Shared.Classes.GeoJson ParseToGeoJson(GeoJsonPoint<GeoJson2DGeographicCoordinates> geoJsonPoint)
        {
            ArgumentNullException.ThrowIfNull(geoJsonPoint);

            return new Shared.Classes.GeoJson()
            {
                coordinates = [geoJsonPoint.Coordinates.Longitude, geoJsonPoint.Coordinates.Latitude]
            };
        }

        public static MongoDB.Driver.GeoJsonObjectModel.GeoJsonPoint<GeoJson2DGeographicCoordinates> ParseToGeoJsonMongoDB(Shared.Classes.GeoJson geoJson)
        {
            ArgumentNullException.ThrowIfNull(geoJson);

            return new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                new GeoJson2DGeographicCoordinates(geoJson.coordinates[0], geoJson.coordinates[1])
            );
        }
    }
}