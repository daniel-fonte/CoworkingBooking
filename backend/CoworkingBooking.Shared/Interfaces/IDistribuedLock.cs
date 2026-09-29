namespace CoworkingBooking.Shared.Interfaces
{
    public interface IDistribuedLock
    {
        Task<string?> AcquireLock(string resource, TimeSpan ttl, TimeSpan wait);
        Task<bool> ReleaseAsync(string resource, string token);
    }
}