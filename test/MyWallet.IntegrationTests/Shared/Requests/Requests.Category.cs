namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Categories
    {
        public static HttpRequestMessage GetCategory(Ulid id) =>
            new(HttpMethod.Get, $"{BasePath}/categories/{id}");

        public static HttpRequestMessage ListCategories() => new(HttpMethod.Get, $"{BasePath}/categories");

        public static HttpRequestMessage CreateCategory(string type, string name, string color)
        {
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

        public static HttpRequestMessage EditCategory(Ulid id, string? name, string? color)
        {
            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/categories/{id}/edit")
            {
                Content = ToJsonStringContent(new
                {
                    name,
                    color
                })
            };
        }

        public static HttpRequestMessage DeleteCategory(Ulid id) =>
            new(HttpMethod.Delete, $"{BasePath}/categories/{id}");
    }
}