using MyWallet.Domain.Categories;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class EditCategoryTests(TestApplicationFactory app) : CategoryIntegrationTest(app)
{
    [Fact]
    public async Task EditCategory_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var (userId, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.EditCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task EditCategory_WhenRequestIsValid_ShouldEditCategory()
    {
        // Arrange
        var (userId, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.EditCategory(categoryId);

        // Act
        await client.SendAsync(request);

        // Assert
        var category = await GetRequiredService<ICategoryRepository>()
            .GetAsync(categoryId);
        
        category.IsError.Should().BeFalse();
        category.Value.Should().BeEquivalentTo(new
        {
            Name = Constants.Category.Name2,
            Color = Constants.Category.Color2
        });
    }

    [Fact]
    public async Task EditCategory_WhenCategoryIsOwnedByAnotherUser_ShouldReturnForbidden()
    {
        // Arrange
        var (_, accessToken) = await CreateUserAsync();
        var (userId, _) = await CreateUser2Async();

        var client = CreateClient(accessToken);

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.EditCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task EditCategory_WhenCategoryDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var (_, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var request = Requests.Categories.EditCategory(CategoryId.New());

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditCategory_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var (userId, _) = await CreateUserAsync();
        var client = CreateClient();

        var categoryId = await CreateCategoryAsync(userId);
        var request = Requests.Categories.EditCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}