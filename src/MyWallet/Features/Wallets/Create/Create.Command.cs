using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Create;

public sealed record CreateWalletCommand : ICommand<WalletId>, IHaveUser
{
    public required string Name { get; init; }

    public required string Color { get; init; }

    public required string Currency { get; init; }

    public Ulid UserId { get; set; }
}