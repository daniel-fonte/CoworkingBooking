using MongoDB.Driver;

namespace CoworkingBooking.Shared.Interfaces
{
    public interface IRepository<TEntity>
    {
        Task<TEntity?> FindOneById(string id);
        Task<TEntity?> UpdateOneById(string id, TEntity entity);
        Task<long> InsertMany(List<TEntity> entities, IClientSessionHandle? session = null);
        Task<List<TEntity>> FindAll();
        Task<TEntity> InsertOne(TEntity workspace);
    }
}