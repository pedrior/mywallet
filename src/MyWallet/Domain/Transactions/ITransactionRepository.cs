using MyWallet.Domain.Wallets;

namespace MyWallet.Domain.Transactions;

public interface ITransactionRepository : IRepository<Transaction, TransactionId>
{
    Task<WalletId?> GetWalletIdAsync(TransactionId transactionId, CancellationToken cancellationToken);
}