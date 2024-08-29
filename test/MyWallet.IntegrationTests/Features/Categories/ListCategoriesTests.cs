using System.Net.Http.Json;
using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.List;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class ListCategoriesTests(TestApplicationFactory app) : CategoryIntegrationTest(app)
{
    private const int CategoriesCount = 5;

    [Fact]
    public async Task ListCategories_WhenRequestIsValid_ShouldReturnOK()
    {
        // Arrange
        var (_, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var request = Requests.Categories.ListCategories();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListCategories_WhenRequestIsValid_ShouldReturnCategories()
    {
        // Arrange
        var (userId, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);
        
        await RemoveAllCategoriesAsync(userId);
        for (var i = 0; i < CategoriesCount; i++)
        {
            await CreateCategoryAsync(userId, id: CategoryId.New());
        }
        
        var request = Requests.Categories.ListCategories();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        var categoriesResponse = await response.Content
            .ReadFromJsonAsync<List<ListCategoriesResponse>>();

        categoriesResponse.Should().NotBeNull();
        categoriesResponse.Should().HaveCount(CategoriesCount);
    }

    [Fact]
    public async Task ListCategories_WhenUserHasNoCategories_ShouldReturnEmptyList()
    {
        // Arrange
        var (userId, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);
        
        await RemoveAllCategoriesAsync(userId);
        
        var request = Requests.Categories.ListCategories();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categoriesResponses = await response.Content
            .ReadFromJsonAsync<List<ListCategoriesResponse>>();

        categoriesResponses.Should().NotBeNull();
        categoriesResponses.Should().BeEmpty();
    }

    [Fact]
    public async Task ListCategories_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();

        var request = Requests.Categories.ListCategories();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}