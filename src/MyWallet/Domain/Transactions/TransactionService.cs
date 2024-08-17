using MyWallet.Domain.Categories;
using MyWallet.Domain.Wallets;

namespace MyWallet.Domain.Transactions;

public sealed class TransactionService : ITransactionService
{
    public ErrorOr<Transaction> CreateTransaction(
        Wallet wallet,
        Category category,
        TransactionType type,
        TransactionName name,
        Amount amount,
        Currency currency,
        DateOnly date)
    {
        if (wallet.IsArchived)
        {
            return TransactionErrors.WalletIsArchived;
        }

        if (!type.MatchCategoryType(category.Type))
        {
            return TransactionErrors.CategoryTypeMismatch;
        }

        // I plan to use a currency conversion service in the future...
        if (wallet.Currency != currency)
        {
            return TransactionErrors.CurrencyMismatch;
        }

        return Transaction.Create(
            id: TransactionId.New(),
            walletId: wallet.Id,
            categoryId: category.Id,
            type: type,
            name: name,
            amount: amount,
            currency: currency,
            date: date);
    }
}