using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Create;
using MyWallet.Features.Categories.Edit;

namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Categories
    {
        public static HttpRequestMessage GetCategory(CategoryId id) =>
            new(HttpMethod.Get, $"{BasePath}/categories/{id}");

        public static HttpRequestMessage ListCategories() => new(HttpMethod.Get, $"{BasePath}/categories");

        public static HttpRequestMessage CreateCategory(CreateCategoryRequest? request = null)
        {
            request ??= new CreateCategoryRequest
            {
                Type = Constants.Category.Type.Name,
                Name = Constants.Category.Name.Value,
                Color = Constants.Category.Color.Value
            };
            
            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/categories")
            {
                Content = ToJsonStringContent(request)
            };
        }

        public static HttpRequestMessage EditCategory(CategoryId id, EditCategoryRequest? request = null)
        {
            request ??= new EditCategoryRequest
            {
                Name = Constants.Category.Name2.Value,
                Color = Constants.Category.Color2.Value
            };
            
            return new HttpRequestMessage(HttpMethod.Patch, $"{BasePath}/categories/{id}")
            {
                Content = ToJsonStringContent(request)
            };
        }

        public static HttpRequestMessage DeleteCategory(CategoryId id) =>
            new(HttpMethod.Delete, $"{BasePath}/categories/{id}");
    }
}