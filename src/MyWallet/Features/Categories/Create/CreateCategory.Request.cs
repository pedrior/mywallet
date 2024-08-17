namespace MyWallet.Features.Categories.Create;

public sealed record CreateCategoryRequest
{
    public required string Type { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }
    
    public CreateCategoryCommand ToCommand() => new()
    {
        Type = Type,
        Name = Name,
        Color = Color
    };
}