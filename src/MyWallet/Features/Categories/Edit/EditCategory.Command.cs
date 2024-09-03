namespace MyWallet.Features.Categories.Edit;

public sealed record EditCategoryCommand : ICommand<Updated>, IHaveUser
{
    public required Ulid CategoryId { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }

    public Ulid UserId { get; set; }
}