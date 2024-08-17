using MyWallet.Domain.Categories;

namespace MyWallet.Domain.Transactions;

public sealed class TransactionType : Enum<TransactionType>
{
    public static readonly TransactionType Income = new("income", 1);
    public static readonly TransactionType Expense = new("expense", 2);

    private TransactionType(string name, int value) : base(name, value)
    {
    }

    public bool MatchCategoryType(CategoryType categoryType) =>
        this == Income && categoryType == CategoryType.Income
        || this == Expense && categoryType == CategoryType.Expense;
}