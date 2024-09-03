using MyWallet.Domain.Categories;
using MyWallet.IntegrationTests.Shared.Extensions;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class CreateCategoryTests(TestApplicationFactory app) : CategoryIntegrationTest(app)
{
    [Fact]
    public async Task CreateCategory_WhenRequestIsValid_ShouldReturnCreated()
    {
        // Arrange
        var (_, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var request = Requests.Categories.CreateCategory();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(
            $"/categories/{response.Headers.FindLastResourceIdentifier()}");
    }

    [Fact]
    public async Task CreateCategory_WhenRequestIsValid_ShouldCreateCategory()
    {
        // Arrange
        var (_, accessToken) = await CreateUserAsync();
        var client = CreateClient(accessToken);

        var request = Requests.Categories.CreateCategory();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        var categoryId = Ulid.Parse(response.Headers.FindLastResourceIdentifier());
        var category = await GetRequiredService<ICategoryRepository>()
            .GetAsync(new CategoryId(categoryId));
        
        category.IsError.Should().BeFalse();
        category.Value.Should().BeEquivalentTo(new
        {
            Constants.Category.Type,
            Constants.Category.Name,
            Constants.Category.Color
        });
    }

    [Fact]
    public async Task CreateCategory_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();

        var request = Requests.Categories.CreateCategory();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}