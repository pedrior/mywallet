using MyWallet.Domain.Categories.ValueObjects;

namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Categories
    {
        public static HttpRequestMessage GetCategory(CategoryId categoryId) =>
            new(HttpMethod.Get, $"{BasePath}/categories/{categoryId}");

        public static HttpRequestMessage ListCategories() => new(HttpMethod.Get, $"{BasePath}/categories");

        public static HttpRequestMessage CreateCategory(
            string? type = null,
            string? name = null,
            string? color = null)
        {
            type ??= Constants.Category.Type.Name;
            name ??= Constants.Category.Name.Value;
            color ??= Constants.Category.Color.Value;

            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/categories")
            {
                Content = ToJsonStringContent(new
                {
                    type,
                    name,
                    color
                })
            };
        }

        public static HttpRequestMessage EditCategory(
            CategoryId categoryId,
            string? name = null,
            string? color = null)
        {
            name ??= Constants.Category.Name2.Value;
            color ??= Constants.Category.Color2.Value;

            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/categories/{categoryId}/edit")
            {
                Content = ToJsonStringContent(new
                {
                    name,
                    color
                })
            };
        }

        public static HttpRequestMessage DeleteCategory(CategoryId categoryId) =>
            new(HttpMethod.Delete, $"{BasePath}/categories/{categoryId}");
    }
}