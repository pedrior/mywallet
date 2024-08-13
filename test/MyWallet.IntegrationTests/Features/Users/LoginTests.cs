using System.Net.Http.Json;
using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Repository;
using MyWallet.Features.Users;

namespace MyWallet.IntegrationTests.Features.Users;

public sealed class LoginTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private User user = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        
        user = (await Factories.User.CreateDefaultWithServiceProvider(Services)).Value;

        await userRepository.AddAsync(user);
    }
    
    [Fact]
    public async Task Login_WhenRequestIsValid_ShouldReturnAccessToken()
    {
        // Arrange
        var request = Requests.Users.Login(
            email: Constants.User.Email.Value,
            password: Constants.User.Password.Value);
        
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        var token = new JsonWebToken(loginResponse!.AccessToken);

        var sub = token.GetClaim(JwtRegisteredClaimNames.Sub).Value;
        var name = token.GetClaim(JwtRegisteredClaimNames.Name).Value;
        var email = token.GetClaim(JwtRegisteredClaimNames.Email).Value;

        sub.Should().Be(user.Id.ToString());
        name.Should().Be(user.Name.Value);
        email.Should().Be(user.Email.Value);
    }

    [Fact]
    public async Task Login_WhenUserDoesNotExist_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Users.Login(
            email: Constants.User.Email2.Value,
            password: Constants.User.Password.Value);
        
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WhenPasswordIsIncorrect_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Users.Login(
            email: Constants.User.Email.Value,
            password: Constants.User.Password2.Value);
        
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}