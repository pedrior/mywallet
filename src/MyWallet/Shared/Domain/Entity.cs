namespace MyWallet.Shared.Domain;

public abstract class Entity : IEntity
{
    private readonly List<IEvent> events = [];

    public IReadOnlyCollection<IEvent> Events => events;

    public void AddEvent(IEvent @event) => events.Add(@event);

    public void ClearEvents() => events.Clear();
}

public abstract class Entity<TId>: Entity, IEntity<TId>, IEquatable<Entity<TId>> where TId : notnull
{
    public abstract TId Id { get; init; }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !Equals(left, right);

    public bool Equals(Entity<TId>? other)
    {
        return !ReferenceEquals(null, other)
               && (ReferenceEquals(this, other) || EqualityComparer<TId>.Default.Equals(Id, other.Id));
    }

    public override bool Equals(object? obj)
    {
        return !ReferenceEquals(null, obj) &&
               (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Entity<TId>)obj));
    }

    public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id);
}