namespace MyWallet.Features.Wallets.List;

public sealed record WalletSummaryResponse
{
    public required Ulid Id { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
}