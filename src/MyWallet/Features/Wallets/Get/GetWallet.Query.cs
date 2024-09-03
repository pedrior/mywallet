namespace MyWallet.Features.Wallets.Get;

public sealed record GetWalletQuery(Ulid WalletId) : IQuery<WalletResponse>, IHaveUser
{
    public Ulid UserId { get; set; }
}