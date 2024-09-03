using MyWallet.Domain.Wallets;

namespace MyWallet.Domain.Transactions;

public interface ITransactionRepository : IRepository<Transaction, TransactionId>
{
    Task<ErrorOr<WalletId>> GetWalletIdAsync(TransactionId transactionId, CancellationToken cancellationToken);
    
    Task DeleteAllByWallet(WalletId walletId, CancellationToken cancellationToken);
}