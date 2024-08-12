using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Domain.Wallets.Repository;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class RenameWalletTests(TestApplicationFactory app) : IntegrationTest(app)
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
    public async Task RenameWallet_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Wallets.RenameWallet(
            walletId,
            Constants.Wallet.Name2.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RenameWallet_WhenRequestIsValid_ShouldChangeWalletName()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Wallets.RenameWallet(
            walletId,
            Constants.Wallet.Name2.Value);

        // Act
        await client.SendAsync(request);

        // Assert
        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new(walletId));

        wallet!.Name.Should().Be(Constants.Wallet.Name2);
    }

    [Fact]
    public async Task RenameWallet_WhenWalletDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingWalletId = Ulid.NewUlid();

        var client = CreateClient(accessToken);
        var request = Requests.Wallets.GetWallet(nonExistingWalletId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RenameWallet_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();
        var request = Requests.Wallets.GetWallet(walletId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RenameWallet_WhenUserDoesNotOwnWallet_ShouldReturnForbidden()
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
        var request = Requests.Wallets.GetWallet(walletId);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}