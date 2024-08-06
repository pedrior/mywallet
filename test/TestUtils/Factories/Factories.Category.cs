using MyWallet.Domain;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.ValueObjects;

namespace TestUtils.Factories;

public static partial class Factories
{
    public static class Category
    {
        public static MyWallet.Domain.Categories.Category CreateDefault(
            CategoryId? id = null,
            UserId? userId = null,
            CategoryName? name = null,
            Color? color = null)
        {
            return MyWallet.Domain.Categories.Category.Create(
                id ?? Constants.Constants.Category.Id,
                userId ?? Constants.Constants.User.Id,
                name ?? Constants.Constants.Category.Name,
                color ?? Constants.Constants.Category.Color);
        }
    }
}