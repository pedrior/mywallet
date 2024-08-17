using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;

namespace TestUtils.Factories;

public static partial class Factories
{
    public static class Transaction
    {
        public static ErrorOr<MyWallet.Domain.Transactions.Transaction> CreateDefault(
            TransactionId? id = null,
            WalletId? walletId = null,
            CategoryId? categoryId = null,
            TransactionType? type = null,
            TransactionName? name = null,
            Amount? amount = null,
            Currency? currency = null,
            DateOnly? date = null)
        {
            return MyWallet.Domain.Transactions.Transaction.Create(
                id ?? Constants.Constants.Transaction.Id,
                walletId ?? Constants.Constants.Wallet.Id,
                categoryId ?? Constants.Constants.Category.Id,
                type ?? Constants.Constants.Transaction.Type,
                name ?? Constants.Constants.Transaction.Name,
                amount ?? Constants.Constants.Transaction.Amount,
                currency ?? Constants.Constants.Transaction.Currency,
                date ?? Constants.Constants.Transaction.Date);
        }
    }
}