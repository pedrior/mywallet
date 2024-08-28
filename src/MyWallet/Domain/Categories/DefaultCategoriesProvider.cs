using MyWallet.Domain.Users;

namespace MyWallet.Domain.Categories;

public sealed class DefaultCategoriesProvider : IDefaultCategoriesProvider
{
    private static readonly Color IncomeCategoryColor = Color.Create("#03CEA4").Value;
    private static readonly Color ExpenseCategoryColor = Color.Create("#FB4D3D").Value;

    public IReadOnlyList<Category> CreateDefaultCategoriesForUser(User user)
    {
        Category[] categories =
        [
            ..CreateDefaultIncomeCategories(user.Id),
            ..CreateDefaultExpenseCategories(user.Id)
        ];

        return categories.AsReadOnly();
    }

    private static Category[] CreateDefaultIncomeCategories(UserId userId) =>
    [
        CreateCategory(userId, CategoryType.Income, "Salary"),
        CreateCategory(userId, CategoryType.Income, "Refund"),
        CreateCategory(userId, CategoryType.Income, "Dividends"),
        CreateCategory(userId, CategoryType.Income, "Freelance"),
        CreateCategory(userId, CategoryType.Income, "Gift"),
        CreateCategory(userId, CategoryType.Income, "Investment"),
        CreateCategory(userId, CategoryType.Income, "Rental"),
        CreateCategory(userId, CategoryType.Income, "Other")
    ];

    private static Category[] CreateDefaultExpenseCategories(UserId userId) =>
    [
        CreateCategory(userId, CategoryType.Expense, "Food"),
        CreateCategory(userId, CategoryType.Expense, "Transport"),
        CreateCategory(userId, CategoryType.Expense, "Health"),
        CreateCategory(userId, CategoryType.Expense, "Entertainment"),
        CreateCategory(userId, CategoryType.Expense, "Shopping"),
        CreateCategory(userId, CategoryType.Expense, "Bills"),
        CreateCategory(userId, CategoryType.Expense, "Education"),
        CreateCategory(userId, CategoryType.Expense, "Housing"),
        CreateCategory(userId, CategoryType.Expense, "Travel"),
        CreateCategory(userId, CategoryType.Expense, "Gift"),
        CreateCategory(userId, CategoryType.Expense, "Insurance"),
        CreateCategory(userId, CategoryType.Expense, "Pets"),
        CreateCategory(userId, CategoryType.Expense, "Investment"),
        CreateCategory(userId, CategoryType.Expense, "Loan"),
        CreateCategory(userId, CategoryType.Expense, "Other")
    ];

    private static Category CreateCategory(UserId userId, CategoryType type, string name) => Category.Create(
        id: CategoryId.New(),
        userId: userId,
        type: type,
        name: CategoryName.Create(name).Value,
        color: type == CategoryType.Income
            ? IncomeCategoryColor
            : ExpenseCategoryColor);
}