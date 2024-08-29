using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;
using MyWallet.Shared.Persistence;

namespace MyWallet.IntegrationTests.Features.Categories;

public abstract class CategoryIntegrationTest(TestApplicationFactory app) : UserAwareIntegrationTests(app)
{
    protected async Task<CategoryId> CreateCategoryAsync(
        UserId userId,
        CategoryId? id = null,
        CategoryType? type = null,
        CategoryName? name = null,
        Color? color = null)
    {
        var category = Factories.Category.CreateDefault(
            id: id,
            userId: userId,
            type: type,
            name: name,
            color: color);

        await GetRequiredService<ICategoryRepository>()
            .AddAsync(category);

        return category.Id;
    }

    protected Task<int> RemoveAllCategoriesAsync(UserId userId)
    {
        return GetRequiredService<IDbContext>()
            .ExecuteAsync(
                sql: """
                         DELETE FROM Categories c
                         WHERE c.user_id = @userId
                     """,
                param: new { userId });
    }
}