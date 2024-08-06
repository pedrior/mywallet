using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;

namespace MyWallet.IntegrationTests.Features.Users;

public sealed class UpdateProfileTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private UserId userId = null!;
    private string accessToken = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        userId = user.Value.Id;
        accessToken = CreateAccessToken(user.Value);
    }

    [Fact]
    public async Task UpdateProfile_WhenUserIsAuthenticated_ShouldUpdateProfile()
    {
        // Arrange
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(Requests.Users.UpdateProfile());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var userRepository = GetRequiredService<IUserRepository>();

        var user = await userRepository.GetAsync(userId);

        user!.Name.Should().Be(Constants.User.Name2);
    }

    [Fact]
    public async Task UpdateProfile_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(Requests.Users.UpdateProfile());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}