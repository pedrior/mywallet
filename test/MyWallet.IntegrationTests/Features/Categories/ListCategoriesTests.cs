using System.Net.Http.Json;
using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Features.Categories;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class ListCategoriesTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private const int DefaultCategoriesCount = 5;
    private string accessToken = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        var categoryRepository = GetRequiredService<ICategoryRepository>();
        
        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);
        
        for (var i = 0; i < DefaultCategoriesCount; i++)
        {
            var category = Factories.Category.CreateDefault(
                id: CategoryId.New(),
                userId: user.Value.Id);

            await categoryRepository.AddAsync(category);
        }
        
        accessToken = CreateAccessToken(user.Value);
    }

    [Fact]
    public async Task ListCategories_WhenUserOwnsCategory_ShouldReturnCategory()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Categories.ListCategories();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categoriesResponses = await response.Content
            .ReadFromJsonAsync<List<ListCategoriesResponse>>();

        categoriesResponses.Should().NotBeNull();
        categoriesResponses.Should().HaveCount(DefaultCategoriesCount);
    }

    [Fact]
    public async Task ListCategories_WhenUserHasNoCategories_ShouldReturnEmptyList()
    {
        // Arrange
        var userRepository = GetRequiredService<IUserRepository>();
        var user = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(user.Value);

        var client = CreateClient(CreateAccessToken(user.Value));
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