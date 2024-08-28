using MyWallet.Domain.Users;

namespace MyWallet.Domain.Categories;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task AddRangeAsync(IEnumerable<Category> categories, CancellationToken cancellationToken = default);
        
    Task<bool> IsOwnedByUserAsync(
        CategoryId categoryId,
        UserId userId,
        CancellationToken cancellationToken = default);
}