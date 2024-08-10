namespace MyWallet.Shared.Domain;

public interface IEntity
{
    IReadOnlyCollection<IEvent> Events { get; }
    
    void AddEvent(IEvent @event);
    
    void ClearEvents();
}

public interface IEntity<out TId> : IEntity where TId : notnull
{
    TId Id { get; }
}
