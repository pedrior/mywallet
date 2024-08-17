using MyWallet.Domain.Categories;
using MyWallet.Domain.Wallets;

namespace MyWallet.Domain.Transactions;

public interface ITransactionService
{
    ErrorOr<Transaction> CreateTransaction(
        Wallet wallet,
        Category category,
        TransactionType type,
        TransactionName name,
        Amount amount,
        Currency currency,
        DateOnly date);
}