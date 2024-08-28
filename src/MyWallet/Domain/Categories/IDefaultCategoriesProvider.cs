using MyWallet.Domain.Users;

namespace MyWallet.Domain.Categories;

public interface IDefaultCategoriesProvider
{
    IReadOnlyList<Category> CreateDefaultCategoriesForUser(User user);
}