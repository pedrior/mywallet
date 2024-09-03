namespace MyWallet.Shared.Domain;

public interface IRepository<TEntity, in TId> where TEntity : IEntity<TId>, IAggregateRoot where TId : notnull
{
    Task<ErrorOr<TEntity>> GetAsync(TId id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
}