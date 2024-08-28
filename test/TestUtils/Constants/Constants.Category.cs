using MyWallet.Domain;
using MyWallet.Domain.Categories;

namespace TestUtils.Constants;

public static partial class Constants
{
    public static class Category
    {
        public static readonly CategoryId Id = CategoryId.New();
        public static readonly CategoryType Type = CategoryType.Expense;
        public static readonly CategoryName Name = CategoryName.Create("Food").Value;
        public static readonly Color Color = Color.Create("#202020").Value;
        
        public static readonly CategoryId Id2 = CategoryId.New();
        public static readonly CategoryType Type2 = CategoryType.Expense;
        public static readonly CategoryName Name2 = CategoryName.Create("Groceries and Snacks").Value;
        public static readonly Color Color2 = Color.Create("#E8B6C5").Value;
    }
}