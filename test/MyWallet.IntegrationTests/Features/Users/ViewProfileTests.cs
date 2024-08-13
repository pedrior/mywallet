using System.Net.Http.Json;
using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Repository;
using MyWallet.Features.Users;

namespace MyWallet.IntegrationTests.Features.Users;

public sealed class ViewProfileTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private User user = null!;
    private string accessToken = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        user = (await Factories.User.CreateDefaultWithServiceProvider(Services)).Value;
        await userRepository.AddAsync(user);

        accessToken = CreateAccessToken(user);
    }

    [Fact]
    public async Task ViewProfile_WhenRequestIsValid_ShouldReturnUserProfileResponse()
    {
        // Arrange
        var request = Requests.Users.ViewProfile();
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userProfileResponse = await response.Content.ReadFromJsonAsync<UserProfileResponse>();

        userProfileResponse!.Name.Should().Be(user.Name.Value);
        userProfileResponse.Email.Should().Be(user.Email.Value);
    }

    [Fact]
    public async Task ViewProfile_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Users.ViewProfile();
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}