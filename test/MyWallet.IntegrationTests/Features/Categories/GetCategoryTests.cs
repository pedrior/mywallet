using System.Net.Http.Json;
using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Features.Categories;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class GetCategoryTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private string accessToken = null!;
    private CategoryId categoryId = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        var categoryRepository = GetRequiredService<ICategoryRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        var category = Factories.Category.CreateDefault(userId: user.Value.Id);
        await categoryRepository.AddAsync(category);

        accessToken = CreateAccessToken(user.Value);
        categoryId = category.Id;
    }

    [Fact]
    public async Task GetCategory_WhenUserOwnsCategory_ShouldReturnCategory()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Categories.GetCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categoryResponse = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        categoryResponse.Should().NotBeNull();

        categoryResponse!.Id.Should().Be(categoryId.Value);
        categoryResponse.Type.Should().Be(Constants.Category.Type.Name);
        categoryResponse.Name.Should().Be(Constants.Category.Name.Value);
        categoryResponse.Color.Should().Be(Constants.Category.Color.Value);
    }

    [Fact]
    public async Task GetCategory_WhenUserDoesNotOwnCategory_ShouldReturnForbidden()
    {
        // Arrange
        var user = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await GetRequiredService<IUserRepository>()
            .AddAsync(user.Value);

        var token = CreateAccessToken(user.Value);
        var client = CreateClient(token);

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
        var client = CreateClient();
        var request = Requests.Categories.GetCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}