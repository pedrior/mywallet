using MyWallet.Domain.Users;

namespace MyWallet.IntegrationTests.Features.Users;

public sealed class ChangeEmailTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private UserId userId = null!;
    private string accessToken = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            email: Constants.User.Email,
            password: Constants.User.Password);

        await userRepository.AddAsync(user.Value);

        userId = user.Value.Id;
        accessToken = CreateAccessToken(user.Value);
    }

    [Fact]
    public async Task ChangeEmail_WhenRequestIsValid_ShouldChangeUserEmail()
    {
        // Arrange
        var request = Requests.Users.ChangeEmail(
            newEmail: Constants.User.Email2.Value,
            password: Constants.User.Password.Value);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var userRepository = GetRequiredService<IUserRepository>();
        var user = await userRepository.GetAsync(userId);

        user.IsError.Should().BeFalse();
        user.Value.Email.Should().Be(Constants.User.Email2);
    }

    [Fact]
    public async Task ChangeEmail_WhenPasswordIsIncorrect_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var request = Requests.Users.ChangeEmail(
            newEmail: Constants.User.Email2.Value,
            password: Constants.User.Password2.Value);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task ChangeEmail_WhenNewEmailIsEqualOldEmail_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var request = Requests.Users.ChangeEmail(
            newEmail: Constants.User.Email.Value,
            password: Constants.User.Password.Value);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task ChangeEmail_WhenNewEmailIsNotUnique_ShouldReturnConflict()
    {
        // Arrange
        var request = Requests.Users.ChangeEmail(
            newEmail: Constants.User.Email2.Value,
            password: Constants.User.Password.Value);

        var client = CreateClient(accessToken);

        // Create another user with the email that we want to change to
        var userRepository = GetRequiredService<IUserRepository>();
        var user = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(user.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ChangeEmail_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Users.ChangeEmail(
            newEmail: Constants.User.Email2.Value,
            password: Constants.User.Password.Value);

        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}