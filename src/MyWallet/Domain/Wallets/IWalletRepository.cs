using MyWallet.Domain.Users;

namespace MyWallet.Domain.Wallets;

public interface IWalletRepository : IRepository<Wallet, WalletId>
{
    Task<bool> IsOwnedByUserAsync(
        WalletId walletId,
        UserId userId,
        CancellationToken cancellationToken);
}