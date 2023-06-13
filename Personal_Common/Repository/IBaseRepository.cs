using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Personal.Common.Model;

namespace Personal.Common.Repository;

public interface IBaseRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity, string userId);

    Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, string userId);

    Task<TEntity> UpdateAsync(TEntity entity, string userId);

    Task UpdateAsync(IEnumerable<TEntity> entities, string userId);

    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, Object>> includes = null);

    Task<PagedList<TEntity>> GetPagedListAsync(int currentPage, int pageSize, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, Object>> includes = null);

    Task<TEntity> GetIdAsync(TKey id, string[] paths = null);

    Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, Object>> includes = null);

    Task<int> DeleteAsync(TEntity entity);

    Task<int> DeleteAsync(IEnumerable<TEntity> entities);
}