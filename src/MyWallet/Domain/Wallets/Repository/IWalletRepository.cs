using MyWallet.Domain.Wallets.ValueObjects;

namespace MyWallet.Domain.Wallets.Repository;

public interface IWalletRepository : IRepository<Wallet, WalletId>;