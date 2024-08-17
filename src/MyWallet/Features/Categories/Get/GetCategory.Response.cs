namespace MyWallet.Features.Categories.Get;

public sealed record CategoryResponse
{
    public required Ulid Id { get; init; }

    public required string Type { get; init; }
    
    public required string Name { get; init; }

    public required string Color { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }
}