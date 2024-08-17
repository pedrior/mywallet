using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class UnarchiveWalletTests(TestApplicationFactory app) : IntegrationTest(app)
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
        wallet.Archive();
        
        await walletRepository.AddAsync(wallet);

        walletId = wallet.Id.Value;
    }

    [Fact]
    public async Task UnarchiveWallet_WhenRequestIsValid_ShouldUnarchiveWallet()
    {
        // Arrange
        var request = Requests.Wallets.UnarchiveWallet(walletId);

        var client = CreateClient(accessToken);
        
        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new WalletId(walletId));

        wallet.Should().NotBeNull();
        wallet!.IsArchived.Should().BeFalse();
    }

    [Fact]
    public async Task UnarchiveWallet_WhenWalletIsAlreadyUnarchived_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new WalletId(walletId));
        wallet!.Unarchive();

        await walletRepository.UpdateAsync(wallet);

        var request = Requests.Wallets.UnarchiveWallet(walletId);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UnarchiveWallet_WhenWalletDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Wallets.UnarchiveWallet(Ulid.NewUlid());

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UnarchiveWallet_WhenUserDoesNotOwnWallet_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        var userRepository = GetRequiredService<IUserRepository>();
        await userRepository.AddAsync(otherUser.Value);

        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var request = Requests.Wallets.UnarchiveWallet(walletId);

        var client = CreateClient(otherUserAccessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UnarchiveWallet_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Wallets.UnarchiveWallet(walletId);
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}