namespace MyWallet.Domain.Users.Events;

public sealed class UserCreatedEvent(UserId userId) : IEvent
{
    public UserId UserId { get; init; } = userId;
}