using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Domain.Wallets.ValueObjects;

namespace MyWallet.Domain.Wallets.Repository;

public interface IWalletRepository : IRepository<Wallet, WalletId>
{
    Task<bool> IsOwnedByUserAsync(
        WalletId walletId,
        UserId userId,
        CancellationToken cancellationToken);
}