namespace MyWallet.Domain.Categories;

public sealed class CategoryId(Ulid id) : EntityId(id)
{
    public static CategoryId New() => new(Ulid.NewUlid());
}