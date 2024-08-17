namespace MyWallet.Features.Categories.List;

public sealed record ListCategoriesResponse
{
    public required Ulid Id { get; init; }

    public required string Type { get; init; }
    
    public required string Name { get; init; }

    public required string Color { get; init; }
}