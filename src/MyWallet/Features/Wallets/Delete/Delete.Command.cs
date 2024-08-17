using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Delete;

public sealed record DeleteWalletCommand(Ulid WalletId) : ICommand<Deleted>, IHaveUser
{
    public Ulid UserId { get; set; }
}