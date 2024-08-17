using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Unarchive;

public sealed record UnarchiveWalletCommand : ICommand<Success>, IHaveUser
{
    public required Ulid WalletId { get; init; }

    public Ulid UserId { get; set; }
}