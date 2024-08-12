using System.Net.Http.Json;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Domain.Wallets;
using MyWallet.Domain.Wallets.Repository;
using MyWallet.Features.Wallets;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class GetWalletTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private string accessToken = null!;
    private Wallet wallet = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        var walletRepository = GetRequiredService<IWalletRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);

        await userRepository.AddAsync(user.Value);

        wallet = Factories.Wallet.CreateDefault(userId: user.Value.Id);
        await walletRepository.AddAsync(wallet);

        accessToken = CreateAccessToken(user.Value);
    }

    [Fact]
    public async Task GetWallet_WhenUserOwnsWallet_ShouldReturnWalletResponse()
    {
        // Arrange
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(Requests.Wallets.GetWallet(wallet.Id.Value));

        // Assert

        var walletResponse = await response.Content.ReadFromJsonAsync<WalletResponse>();
        walletResponse.Should().BeEquivalentTo(new
        {
            Id = wallet.Id.Value,
            Name = wallet.Name.Value,
            Color = wallet.Color.Value
        });
    }

    [Fact]
    public async Task GetWallet_WhenWalletDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var nonExistingWalletId = Ulid.NewUlid();

        // Act
        var response = await client.SendAsync(Requests.Wallets.GetWallet(nonExistingWalletId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetWallet_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(Requests.Wallets.GetWallet(wallet.Id.Value));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetWallet_WhenUserDoesNotOwnWallet_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        var userRepository = GetRequiredService<IUserRepository>();

        await userRepository.AddAsync(otherUser.Value);
        
        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var client = CreateClient(otherUserAccessToken);

        // Act
        var response = await client.SendAsync(Requests.Wallets.GetWallet(wallet.Id.Value));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}