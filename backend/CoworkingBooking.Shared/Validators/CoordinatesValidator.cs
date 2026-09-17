using CoworkingBooking.Shared.Classes;

namespace CoworkingBooking.Shared
{
    public class CoordinatesValidator
    {
        public static bool IsValidCoordinates(Coordinates coordinates)
        {
            if(coordinates.lat >= -90.0 && coordinates.lat <= 90.0 && coordinates.lgn >= -180.0 && coordinates.lgn <= 180.0)
            {
                Console.WriteLine("teste");
                return true;
            }

            return false;
        }
    }
}