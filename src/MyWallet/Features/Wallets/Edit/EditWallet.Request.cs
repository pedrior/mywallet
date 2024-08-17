namespace MyWallet.Features.Wallets.Edit;

public sealed record EditWalletRequest
{
    public required string Name { get; init; }

    public required string Color { get; init; }

    public required string Currency { get; init; }

    public EditWalletCommand ToCommand(Ulid walletId) => new()
    {
        WalletId = walletId,
        Name = Name,
        Color = Color,
        Currency = Currency
    };
}