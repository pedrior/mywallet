namespace MyWallet.Domain.Wallets.Events;

public sealed class WalletDeletedEvent(WalletId walletId) : IEvent
{
    public WalletId WalletId { get; init; } = walletId;
}