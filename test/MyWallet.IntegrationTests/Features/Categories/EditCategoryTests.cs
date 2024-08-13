using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class EditCategoryTests(TestApplicationFactory app) : IntegrationTest(app)
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
    public async Task EditCategory_WhenRequestIsValid_ShouldEditCategory()
    {
        // Arrange
        var request = Requests.Categories.EditCategory(
            categoryId.Value,
            name: Constants.Category.Name2.Value,
            color: Constants.Category.Color2.Value);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var categoryRepository = GetRequiredService<ICategoryRepository>();
        var category = await categoryRepository.GetAsync(categoryId);

        category!.Name.Should().Be(Constants.Category.Name2);
        category.Color.Should().Be(Constants.Category.Color2);
    }

    [Fact]
    public async Task EditCategory_WhenUserDoesNotOwnCategory_ShouldReturnForbidden()
    {
        // Arrange
        var userRepository = GetRequiredService<IUserRepository>();
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherAccessToken = CreateAccessToken(otherUser.Value);

        var request = Requests.Categories.EditCategory(
            categoryId.Value,
            name: Constants.Category.Name2.Value,
            color: Constants.Category.Color2.Value);

        var client = CreateClient(otherAccessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task EditCategory_WhenCategoryDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Categories.EditCategory(
            Ulid.NewUlid(),
            name: Constants.Category.Name2.Value,
            color: Constants.Category.Color2.Value);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditCategory_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Categories.EditCategory(
            categoryId.Value,
            name: Constants.Category.Name2.Value,
            color: Constants.Category.Color2.Value);

        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}