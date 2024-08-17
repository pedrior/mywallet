namespace MyWallet.Features.Categories.Edit;

public sealed record EditCategoryRequest
{
    public required string Name { get; init; }

    public required string Color { get; init; }

    public EditCategoryCommand ToCommand(Ulid categoryId) => new()
    {
        CategoryId = categoryId,
        Name = Name,
        Color = Color
    };
}