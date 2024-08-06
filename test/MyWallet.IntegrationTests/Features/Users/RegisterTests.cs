using MyWallet.Domain.Users.Repository;

namespace MyWallet.IntegrationTests.Features.Users;

public sealed class RegisterTests(TestApplicationFactory app) : IntegrationTest(app)
{
    [Fact]
    public async Task Register_WhenRequestIsValid_ShouldReturnCreated()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(Requests.Users.Register());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var userRepository = GetRequiredService<IUserRepository>();
        
        var user = await userRepository.GetByEmailAsync(Constants.User.Email);

        user.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WhenEmailAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        var userRepository = GetRequiredService<IUserRepository>();
        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);
        
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(Requests.Users.Register());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}