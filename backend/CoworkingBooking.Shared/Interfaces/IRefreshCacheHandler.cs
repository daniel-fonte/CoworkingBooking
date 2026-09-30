namespace CoworkingBooking.Shared.Interfaces
{
    public interface IRefreshCacheHandler
    {
        string CacheType { get; }

        Task Execute(string cacheKey, CancellationToken cancellationToken);
    }
}