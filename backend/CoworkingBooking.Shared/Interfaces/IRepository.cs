using System.Linq.Expressions;
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
        Task<long> DeleteMany<TField>(
            Expression<Func<TEntity, TField>> field,
            TField value
        );
    }
}