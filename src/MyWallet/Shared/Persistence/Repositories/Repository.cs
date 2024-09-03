namespace MyWallet.Shared.Persistence.Repositories;

public abstract class Repository<TEntity, TId>(IDbContext context, IPublisher publisher)
    : IRepository<TEntity, TId> where TEntity : IEntity<TId>, IAggregateRoot where TId : notnull
{
    protected IDbContext Context => context;

    public abstract Task<ErrorOr<TEntity>> GetAsync(TId id, CancellationToken cancellationToken = default);

    public abstract Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);

    public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        PublishEventsAsync(entity, cancellationToken);

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        PublishEventsAsync(entity, cancellationToken);

    public abstract Task DeleteAsync(TId id, CancellationToken cancellationToken = default);

    protected async Task PublishEventsAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity.Events.Count is 0)
        {
            return;
        }

        var events = entity.Events.ToArray();
        entity.ClearEvents();

        foreach (var @event in events)
        {
            await publisher.Publish(@event, cancellationToken);
        }
    }
}