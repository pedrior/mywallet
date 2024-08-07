using MyWallet.Domain;
using MyWallet.Domain.Categories.Enums;
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
            CategoryType? type = null,
            CategoryName? name = null,
            Color? color = null)
        {
            return MyWallet.Domain.Categories.Category.Create(
                id ?? Constants.Constants.Category.Id,
                userId ?? Constants.Constants.User.Id,
                type ?? Constants.Constants.Category.Type,
                name ?? Constants.Constants.Category.Name,
                color ?? Constants.Constants.Category.Color);
        }
    }
}