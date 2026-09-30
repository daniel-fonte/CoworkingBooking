namespace CoworkingBooking.Shared.Interfaces
{
    public interface ICacheRepository<T>
    {
        Task<T?> GetByKey(string key, Func<string, Task<T?>> resolveDataFunction);
        Task UpdateByKey(string key, string data);
    }
}