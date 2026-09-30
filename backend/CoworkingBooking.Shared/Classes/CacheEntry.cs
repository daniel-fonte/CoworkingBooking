namespace CoworkingBooking.Shared.Classes
{
    public class CacheEntry<T>
    {
        public required T Data { get; set; }
        public long CreatedAt { get; set; }
    }
}