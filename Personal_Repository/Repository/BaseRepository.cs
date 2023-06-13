using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Personal.Common.Entity;
using Personal.Common.Model;
using Personal.Common.Repository;
using Personal.Repository.Share;
using System.Linq.Expressions;

namespace Personal.Repository.Repository;

public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : BaseEntity
{
    protected readonly AppDbContext appDbContext;
    private readonly DbSet<TEntity> dbSet;

    public BaseRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
        dbSet = appDbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, string userId)
    {
        entity.CreateBy = userId;
        entity.CreateTime = DateTime.Now;
        entity.UpdatedBy = userId;
        entity.UpdatedTime = DateTime.Now;
        var result = await appDbContext.AddAsync<TEntity>(entity);
        await appDbContext.SaveChangesAsync();
        return result.Entity;
    }

    public virtual async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, string userId)
    {
        if (entities == null || !entities.Any())
        {
            return entities;
        }
        entities = entities.Select(e =>
        {
            e.CreateBy = userId;
            e.CreateTime = DateTime.Now;
            e.UpdatedBy = userId;
            e.UpdatedTime = DateTime.Now;
            return e;
        });
        await appDbContext.AddRangeAsync(entities);
        await appDbContext.SaveChangesAsync();
        return entities;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, string userId)
    {
        entity.UpdatedBy = userId;
        entity.UpdatedTime = DateTime.Now;
        var entry = appDbContext.Entry<TEntity>(entity);
        if (entry.State == EntityState.Detached)
        {
            appDbContext.Attach<TEntity>(entity);
        }
        var result = appDbContext.Update<TEntity>(entity);
        foreach (var p in result.Properties)
        {
            if (!p.Metadata.IsPrimaryKey() && !p.IsModified)
            {
                p.IsModified = true;
            }
        }
        await appDbContext.SaveChangesAsync();
        return result.Entity;
    }

    public virtual async Task<PagedList<TEntity>> GetPagedListAsync(int currentPage, int pageSize, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, Object>> includes = null)
    {
        IQueryable<TEntity> query = GetQueryList(filter, orderBy, includes);
        int totalCount = await query.CountAsync();
        var dataList = await query.Skip(currentPage * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<TEntity>(dataList, currentPage, pageSize, totalCount);
    }

    public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, Object>> includes = null)
    {
        IQueryable<TEntity> query = GetQueryList(filter, orderBy, includes);
        return await query.ToListAsync();
    }

    public virtual async Task<TEntity> GetIdAsync(TKey id, string[] paths = null)
    {
        TEntity entity = await dbSet.FindAsync(id);
        foreach (var path in paths)
        {
            appDbContext.Entry(entity).Reference(path).Load();
        }
        return entity;
    }

    public virtual async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, Object>> includes = null)
    {
        IQueryable<TEntity> query = GetQueryList(filter, null, includes);
        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task UpdateAsync(IEnumerable<TEntity> entities, string userId)
    {
        if (entities == null || !entities.Any())
        {
            throw new ArgumentNullException("entities is null");
        }
        entities = entities.Select(e =>
        {
            e.UpdatedBy = userId;
            e.UpdatedTime = DateTime.Now;
            return e;
        });

        dbSet.UpdateRange(entities);
        await appDbContext.SaveChangesAsync();
    }

    public virtual async Task<int> DeleteAsync(TEntity entity)
    {
        if (appDbContext.Entry(entity).State == EntityState.Detached)
        {
            dbSet.Attach(entity);
        }
        dbSet.Remove(entity);
        return await appDbContext.SaveChangesAsync();
    }

    public virtual async Task<int> DeleteAsync(TKey id)
    {
        TEntity entity = await dbSet.FindAsync(id);
        return await DeleteAsync(entity);
    }

    public virtual async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
    {
        if (!entities.Any())
            throw new ArgumentNullException("entities");
        dbSet.RemoveRange(entities);
        return await appDbContext.SaveChangesAsync();
    }

    private IQueryable<TEntity> GetQueryList(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, Object>> includes = null)
    {
        IQueryable<TEntity> query = appDbContext.Set<TEntity>().AsNoTracking();
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        if (includes != null)
        {
            query = includes(query);
        }

        return query;
    }
}