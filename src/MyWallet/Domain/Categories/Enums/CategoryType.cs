namespace MyWallet.Domain.Categories.Enums;

public sealed class CategoryType : Enum<CategoryType>
{
    public static readonly CategoryType Income = new("income", 1);
    public static readonly CategoryType Expense = new("expense", 2);
    
    private CategoryType(string name, int value) : base(name, value)
    {
    }
}