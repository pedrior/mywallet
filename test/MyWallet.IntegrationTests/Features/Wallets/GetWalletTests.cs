using System.Net.Http.Json;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets;
using MyWallet.Features.Wallets.Get;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class GetWalletTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private string accessToken = null!;
    private Ulid walletId;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        var walletRepository = GetRequiredService<IWalletRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        accessToken = CreateAccessToken(user.Value);

        var wallet = Factories.Wallet.CreateDefault(userId: user.Value.Id);
        await walletRepository.AddAsync(wallet);

        walletId = wallet.Id.Value;
    }

    [Fact]
    public async Task GetWallet_WhenRequestIsValid_ShouldReturnWalletResponse()
    {
        // Arrange
        var request = Requests.Wallets.GetWallet(walletId);
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert

        var walletResponse = await response.Content.ReadFromJsonAsync<WalletResponse>();
        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new(walletId));

        walletResponse.Should().BeEquivalentTo(new
        {
            Id = wallet!.Id.Value,
            Name = wallet.Name.Value,
            Color = wallet.Color.Value
        });
    }

    [Fact]
    public async Task GetWallet_WhenWalletDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Wallets.GetWallet(Ulid.NewUlid());
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetWallet_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Wallets.GetWallet(walletId);
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

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

        var request = Requests.Wallets.GetWallet(walletId);

        var client = CreateClient(otherUserAccessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}