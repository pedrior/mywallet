using MyWallet.Domain.Users;

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
    public async Task UpdateProfile_WhenRequestIsValid_ShouldUpdateProfile()
    {
        // Arrange
        var request = Requests.Users.UpdateProfile(
            name: Constants.User.Name2.Value);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var userRepository = GetRequiredService<IUserRepository>();
        var user = await userRepository.GetAsync(userId);

        user.IsError.Should().BeFalse();
        user.Value.Name.Should().Be(Constants.User.Name2);
    }

    [Fact]
    public async Task UpdateProfile_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Users.UpdateProfile(
            name: Constants.User.Name2.Value);

        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}