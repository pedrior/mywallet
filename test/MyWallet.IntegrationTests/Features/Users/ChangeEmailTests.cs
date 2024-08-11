using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;

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
        var client = CreateClient(accessToken);
        var request = Requests.Users.ChangeEmail();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var userRepository = GetRequiredService<IUserRepository>();
        var user = await userRepository.GetAsync(userId);

        user!.Email.Should().Be(Constants.User.Email2);
    }

    [Fact]
    public async Task ChangeEmail_WhenPasswordIsIncorrect_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Users.ChangeEmail(
            password: Constants.User.Password2.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task ChangeEmail_WhenNewEmailIsEqualOldEmail_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Users.ChangeEmail(
            newEmail: Constants.User.Email.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task ChangeEmail_WhenNewEmailIsNotUnique_ShouldReturnConflict()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Users.ChangeEmail();

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
        var client = CreateClient();
        var request = Requests.Users.ChangeEmail();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}