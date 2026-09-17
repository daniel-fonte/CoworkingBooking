namespace CoworkingBooking.Shared.Classes
{
    public class GeoJson
    {
        public string type { get; set; } = "Point";
        public double[] coordinates { get; set; } = [];
    }
}