namespace MyWallet.Domain.Users;

public sealed class UserId(Ulid id) : EntityId(id)
{
    public static UserId New() => new(Ulid.NewUlid());
}