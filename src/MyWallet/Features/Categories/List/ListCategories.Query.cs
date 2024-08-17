using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.List;

public sealed record ListCategoriesQuery : IQuery<List<ListCategoriesResponse>>, IHaveUser
{
    public Ulid UserId { get; set; }
}