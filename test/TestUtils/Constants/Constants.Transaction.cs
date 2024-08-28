using MyWallet.Domain;
using MyWallet.Domain.Transactions;

namespace TestUtils.Constants;

public static partial class Constants
{
    public static class Transaction
    {
        public static readonly TransactionId Id = TransactionId.New();
        public static readonly TransactionType Type = TransactionType.Expense;
        public static readonly TransactionName Name = TransactionName.Create("Jantar com amigos").Value;
        public static readonly Amount Amount = Amount.Create(100).Value;
        public static readonly Currency Currency = Currency.BRL;
        public static readonly DateOnly Date = DateOnly.FromDateTime(DateTime.Now.Date);

        public static readonly TransactionName Name2 = TransactionName.Create("Dinner with friends").Value;
        public static readonly Amount Amount2 = Amount.Create(20).Value;
        public static readonly Currency Currency2 = Currency.USD;
        public static readonly DateOnly Date2 = DateOnly.FromDateTime(DateTime.Now.Date.AddDays(-2));
    }
}