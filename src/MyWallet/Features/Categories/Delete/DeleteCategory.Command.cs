namespace MyWallet.Features.Categories.Delete;

public sealed record DeleteCategoryCommand : ICommand<Deleted>, IHaveUser
{
    public required Ulid CategoryId { get; init; }

    public Ulid UserId { get; set; }
}