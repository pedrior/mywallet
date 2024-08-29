using MyWallet.Domain.Categories;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class DeleteCategoryTests(TestApplicationFactory app) : CategoryIntegrationTest(app)
{
    [Fact]
    public async Task DeleteCategory_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var (userId, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.DeleteCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategory_WhenRequestIsValid_ShouldDeleteCategory()
    {
        // Arrange
        var (userId, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.DeleteCategory(categoryId);

        // Act
        await client.SendAsync(request);

        // Assert
        var category = await GetRequiredService<ICategoryRepository>()
            .GetAsync(categoryId);

        category.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCategory_WhenCategoryIsOwnedByAnotherUser_ShouldReturnForbidden()
    {
        // Arrange
        var (_, accessToken) = await CreateUserAsync();
        var (userId, _) = await CreateUser2Async();

        var client = CreateClient(accessToken);

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.DeleteCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCategory_WhenCategoryDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var (_, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var request = Requests.Categories.DeleteCategory(CategoryId.New());

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategory_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var (userId, _) = await CreateUserAsync();
        var client = CreateClient();

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.DeleteCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}