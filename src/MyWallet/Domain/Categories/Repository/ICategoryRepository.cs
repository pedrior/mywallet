using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.ValueObjects;

namespace MyWallet.Domain.Categories.Repository;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task<bool> IsOwnedByUserAsync(
        CategoryId categoryId,
        UserId userId,
        CancellationToken cancellationToken = default);
}