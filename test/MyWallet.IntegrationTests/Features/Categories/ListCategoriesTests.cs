using System.Net.Http.Json;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;
using MyWallet.Features.Categories.List;
using MyWallet.Shared.Persistence;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class ListCategoriesTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private const int CategoriesCount = 5;

    private string accessToken = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        var categoryRepository = GetRequiredService<ICategoryRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        accessToken = CreateAccessToken(user.Value);

        // Remove any default category
        var db = GetRequiredService<IDbContext>();
        await db.ExecuteAsync(
            sql: """
                     DELETE FROM Categories c
                     WHERE c.user_id = @user_id
                 """,
            param: new { user_id = user.Value.Id });

        // Create categories
        for (var i = 0; i < CategoriesCount; i++)
        {
            var category = Factories.Category.CreateDefault(
                id: CategoryId.New(),
                userId: user.Value.Id);

            await categoryRepository.AddAsync(category);
        }
    }

    [Fact]
    public async Task ListCategories_WhenRequestIsValid_ShouldReturnCategories()
    {
        // Arrange
        var request = Requests.Categories.ListCategories();

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categoriesResponse = await response.Content
            .ReadFromJsonAsync<List<ListCategoriesResponse>>();

        categoriesResponse.Should().NotBeNull();
        categoriesResponse.Should().HaveCount(CategoriesCount);
    }

    [Fact]
    public async Task ListCategories_WhenUserHasNoCategories_ShouldReturnEmptyList()
    {
        // Arrange
        var userRepository = GetRequiredService<IUserRepository>();
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherAccessToken = CreateAccessToken(otherUser.Value);

        // Remove any default category
        var db = GetRequiredService<IDbContext>();
        await db.ExecuteAsync(
            sql: """
                     DELETE FROM Categories c
                     WHERE c.user_id = @user_id
                 """,
            param: new { user_id = otherUser.Value.Id });
        
        var request = Requests.Categories.ListCategories();

        var client = CreateClient(otherAccessToken);

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
        var request = Requests.Categories.ListCategories();

        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}