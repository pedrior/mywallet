using MyWallet.Domain.Users;

namespace MyWallet.IntegrationTests.Features.Users;

public sealed class RegisterTests(TestApplicationFactory app) : IntegrationTest(app)
{
    [Fact]
    public async Task Register_WhenRequestIsValid_ShouldCreateUser()
    {
        // Arrange
        var request = Requests.Users.Register(
            name: Constants.User.Name.Value,
            email: Constants.User.Email.Value,
            password: Constants.User.Password.Value);

        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var userRepository = GetRequiredService<IUserRepository>();
        var user = await userRepository.GetByEmailAsync(Constants.User.Email);

        user.IsError.Should().BeFalse();
        user.Value.Should().BeEquivalentTo(new
        {
            Constants.User.Name,
            Constants.User.Email
        });
    }

    [Fact]
    public async Task Register_WhenEmailAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        var userRepository = GetRequiredService<IUserRepository>();

        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            email: Constants.User.Email);

        await userRepository.AddAsync(otherUser.Value);

        var request = Requests.Users.Register(
            name: Constants.User.Name.Value,
            email: Constants.User.Email.Value,
            password: Constants.User.Password.Value);

        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}