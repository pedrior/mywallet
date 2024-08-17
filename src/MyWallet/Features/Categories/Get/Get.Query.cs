using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Get;

public sealed record GetCategoryQuery(Ulid CategoryId) : IQuery<CategoryResponse>, IHaveUser
{
    public Ulid UserId { get; set; }
}