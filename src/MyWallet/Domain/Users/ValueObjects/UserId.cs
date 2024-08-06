namespace MyWallet.Domain.Users.ValueObjects;

public sealed class UserId(Ulid id) : EntityId(id)
{
    public static UserId New() => new(Ulid.NewUlid());
}