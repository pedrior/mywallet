using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Domain.Wallets.Repository;
using MyWallet.Domain.Wallets.ValueObjects;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class DeleteWalletTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private string accessToken = null!;
    private Ulid walletId;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        var walletRepository = GetRequiredService<IWalletRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);

        await userRepository.AddAsync(user.Value);

        var wallet = Factories.Wallet.CreateDefault(userId: user.Value.Id);
        await walletRepository.AddAsync(wallet);

        walletId = wallet.Id.Value;

        accessToken = CreateAccessToken(user.Value);
    }

    [Fact]
    public async Task DeleteWallet_WhenUserOwnsWallet_ShouldDeleteWallet()
    {
        // Arrange
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(Requests.Wallets.DeleteWallet(walletId));

        // Assert
        response.EnsureSuccessStatusCode();

        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new WalletId(walletId));
        wallet.Should().BeNull();
    }

    [Fact]
    public async Task DeleteWallet_WhenWalletDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var nonExistingWalletId = Ulid.NewUlid();

        // Act
        var response = await client.SendAsync(Requests.Wallets.DeleteWallet(nonExistingWalletId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteWallet_WhenUserDoesNotOwnWallet_ShouldReturnForbidden()
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
        var response = await client.SendAsync(Requests.Wallets.DeleteWallet(walletId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteWallet_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(Requests.Wallets.DeleteWallet(walletId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}