using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;

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

        accessToken = CreateAccessToken(user.Value);

        var category = Factories.Category.CreateDefault(userId: user.Value.Id);
        await categoryRepository.AddAsync(category);

        categoryId = category.Id;
    }
    
    [Fact]
    public async Task DeleteCategory_WhenRequestIsValid_ShouldDeleteCategory()
    {
        // Arrange
        var request = Requests.Categories.DeleteCategory(categoryId.Value);
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var categoryRepository = GetRequiredService<ICategoryRepository>();
        var category = await categoryRepository.GetAsync(categoryId);

        category.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCategory_WhenUserDoesNotOwnCategory_ShouldReturnForbidden()
    {
        // Arrange
        var userRepository = GetRequiredService<IUserRepository>();
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherAccessToken = CreateAccessToken(otherUser.Value);

        var request = Requests.Categories.DeleteCategory(categoryId.Value);

        var client = CreateClient(otherAccessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCategory_WhenCategoryDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Categories.DeleteCategory(Ulid.NewUlid());

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategory_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Categories.DeleteCategory(categoryId.Value);
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}