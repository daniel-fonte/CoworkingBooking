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
        Task<List<TEntity>> FindMany<TField>(
            Expression<Func<TEntity, TField>> field,
            TField value,
            CancellationToken cancellationToken = default
        );
        Task<CursorPaginationRecordResponse<TEntity>> CursorPagination<TField>(
            Expression<Func<TEntity, TField>> field,
            TField value,
            string? cursor,
            int limit = 10,
            CancellationToken cancellationToken = default
        );
    }
}