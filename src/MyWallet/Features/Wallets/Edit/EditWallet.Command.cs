namespace MyWallet.Features.Wallets.Edit;

public sealed record EditWalletCommand : ICommand<Updated>, IHaveUser
{
    public required Ulid WalletId { get; init; }

    public required string Name { get; init; }
    
    public required string Color { get; init; }
    
    public required string Currency { get; init; }

    public Ulid UserId { get; set; }
}