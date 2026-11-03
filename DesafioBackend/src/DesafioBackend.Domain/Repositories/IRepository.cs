using DesafioBackend.Domain.Common;

namespace DesafioBackend.Domain.Repositories;

public interface IRepository<TEntity, TId> where TEntity : BaseEntity<TId>, IAggregateRoot
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}

