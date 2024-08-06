using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class DeleteCategoryTests(TestApplicationFactory app) : IntegrationTest(app)
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

        user.Value.AddCategory(category.Id);
        await userRepository.UpdateAsync(user.Value);

        accessToken = CreateAccessToken(user.Value);
        categoryId = category.Id;
    }

    [Fact]
    public async Task DeleteCategory_WhenUserOwnsCategory_ShouldReturnDeleteCategory()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Categories.DeleteCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var category = await GetRequiredService<ICategoryRepository>()
            .GetAsync(categoryId);

        category.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCategory_WhenUserDoesNotOwnCategory_ShouldReturnForbidden()
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
        var client = CreateClient();
        var request = Requests.Categories.DeleteCategory(categoryId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}