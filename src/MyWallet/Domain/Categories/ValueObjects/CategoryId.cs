namespace MyWallet.Domain.Categories.ValueObjects;

public sealed class CategoryId(Ulid id) : EntityId(id)
{
    public static CategoryId New() => new(Ulid.NewUlid());
}