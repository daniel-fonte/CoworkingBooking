namespace CoworkingBooking.Shared.Events
{
    public sealed record RefreshCacheEvent(
        string cacheType,
        string cacheKey
    );
}