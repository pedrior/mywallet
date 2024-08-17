using MyWallet.Domain.Categories;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Create;

public sealed record CreateCategoryCommand : ICommand<CategoryId>, IHaveUser
{
    public required string Type { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }

    public Ulid UserId { get; set; }
}