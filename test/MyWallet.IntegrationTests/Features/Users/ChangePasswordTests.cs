using MyWallet.Domain.Users;

namespace MyWallet.IntegrationTests.Features.Users;

public sealed class ChangePasswordTests(TestApplicationFactory app) : IntegrationTest(app)
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
    public async Task ChangePassword_WhenRequestIsValid_ShouldChangePassword()
    {
        // Arrange
        var request = Requests.Users.ChangePassword(
            oldPassword: Constants.User.Password.Value,
            newPassword: Constants.User.Password2.Value);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var userRepository = GetRequiredService<IUserRepository>();
        var passwordHasher = GetRequiredService<IPasswordHasher>();

        var user = await userRepository.GetAsync(userId);

        // Check if the new user password is correct
        var verifyPasswordResult = user!.VerifyPassword(Constants.User.Password2, passwordHasher);
        verifyPasswordResult.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Users.ChangePassword(
            oldPassword: Constants.User.Password.Value,
            newPassword: Constants.User.Password2.Value);

        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_WhenOldPasswordIsIncorrect_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var request = Requests.Users.ChangePassword(
            oldPassword: Constants.User.Password2.Value,
            newPassword: Constants.User.Password.Value);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task ChangePassword_WhenPasswordsAreEqual_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var request = Requests.Users.ChangePassword(
            oldPassword: Constants.User.Password.Value,
            newPassword: Constants.User.Password.Value);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}