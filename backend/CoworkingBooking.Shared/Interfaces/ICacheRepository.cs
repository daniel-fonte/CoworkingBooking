namespace CoworkingBooking.Shared.Interfaces
{
    public interface ICacheRepository<TEntity, TCache>
    {
        Task<TEntity?> GetByKey(
            string key,
            Func<string, Task<TEntity?>> resolveDataFunction,
            Func<TCache, TEntity> mapToEntity
        );
        Task UpdateByKey(string key, string data);
        Task DeleteByKey(string key);
    }
}