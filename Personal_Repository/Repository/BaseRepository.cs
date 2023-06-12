using Microsoft.EntityFrameworkCore;
using Personal.Common.Entity;
using Personal.Repository.Share;

namespace Personal.Repository.Repository;

public abstract class BaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly AppDbContext appDbContext;

    public BaseRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
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

    public virtual async Task<IEnumerable<TEntity>> GetListAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        IQueryable<TEntity> query = appDbContext.Set<T>().AsNoTracking().Where(e => !e.IsDeleted);
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return await query.ToListAsync();
    }
}