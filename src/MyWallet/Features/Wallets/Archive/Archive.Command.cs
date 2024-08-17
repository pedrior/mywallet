using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Archive;

public sealed record ArchiveWalletCommand : ICommand<Success>, IHaveUser
{
    public required Ulid WalletId { get; init; }

    public Ulid UserId { get; set; }
}