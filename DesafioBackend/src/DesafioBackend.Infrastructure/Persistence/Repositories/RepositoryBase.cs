using DesafioBackend.Domain.Common;
using DesafioBackend.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId> 
    where TEntity : BaseEntity<TId>, IAggregateRoot
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected RepositoryBase(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Id!.Equals(id), cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}

