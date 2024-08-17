namespace MyWallet.Features.Wallets.Get;

public sealed record WalletResponse
{
    public required Ulid Id { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset? UpdatedAt { get; init; }
}