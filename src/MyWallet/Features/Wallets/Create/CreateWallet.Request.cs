namespace MyWallet.Features.Wallets.Create;

public sealed record CreateWalletRequest
{
    public required string Name { get; init; }

    public required string Color { get; init; }

    public required string Currency { get; init; }

    public CreateWalletCommand ToCommand() => new()
    {
        Name = Name,
        Color = Color,
        Currency = Currency
    };
}