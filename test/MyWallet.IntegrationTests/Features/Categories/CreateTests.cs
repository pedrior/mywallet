using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;

namespace MyWallet.IntegrationTests.Features.Categories;

public sealed class CreateCategoryTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private string accessToken = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        accessToken = CreateAccessToken(user.Value);
    }

    [Fact]
    public async Task CreateCategory_WhenRequestIsValid_ShouldCreateCategory()
    {
        // Arrange
        var request = CreateDefaultRequest();
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Get the category id from the response location header
        var categoryId = response.Headers.Location!
            .ToString()
            .Split('/')
            .Last();

        var categoryRepository = GetRequiredService<ICategoryRepository>();
        var category = await categoryRepository.GetAsync(new(Ulid.Parse(categoryId)));

        category.Should().NotBeNull();
        category!.Type.Should().Be(Constants.Category.Type);
        category.Name.Should().Be(Constants.Category.Name);
        category.Color.Should().Be(Constants.Category.Color);
    }

    [Fact]
    public async Task CreateCategory_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = CreateDefaultRequest();
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static HttpRequestMessage CreateDefaultRequest() =>
        Requests.Categories.CreateCategory(
            type: Constants.Category.Type.Name,
            name: Constants.Category.Name.Value,
            color: Constants.Category.Color.Value);
}