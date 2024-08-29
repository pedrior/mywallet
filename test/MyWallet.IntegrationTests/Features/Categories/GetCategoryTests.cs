using System.Net.Http.Json;
using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Get;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class GetCategoryTests(TestApplicationFactory app) : CategoryIntegrationTest(app)
{
    [Fact]
    public async Task GetCategory_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var (userId, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.GetCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCategory_WhenRequestIsValid_ShouldReturnCategory()
    {
        // Arrange
        var (userId, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.GetCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        var categoryResponse = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        categoryResponse.Should().NotBeNull();

        categoryResponse!.Id.Should().Be(categoryId.Value);
        categoryResponse.Type.Should().Be(Constants.Category.Type.Name);
        categoryResponse.Name.Should().Be(Constants.Category.Name.Value);
        categoryResponse.Color.Should().Be(Constants.Category.Color.Value);
    }

    [Fact]
    public async Task GetCategory_WhenCategoryIsOwnedByAnotherUser_ShouldReturnForbidden()
    {
        // Arrange
        var (_, accessToken) = await CreateUserAsync();
        var (userId, _) = await CreateUser2Async();

        var client = CreateClient(accessToken);

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.GetCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCategory_WhenCategoryDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var (_, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var request = Requests.Categories.GetCategory(CategoryId.New());

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCategory_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var (userId, _) = await CreateUserAsync();
        var client = CreateClient();

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.GetCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}