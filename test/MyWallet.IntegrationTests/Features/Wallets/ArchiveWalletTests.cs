using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class ArchiveWalletTests(TestApplicationFactory app) : IntegrationTest(app)
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
    public async Task ArchiveWallet_WhenRequestIsValid_ShouldArchiveWallet()
    {
        // Arrange
        var request = Requests.Wallets.ArchiveWallet(walletId);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new WalletId(walletId));

        wallet.IsError.Should().BeFalse();
        wallet.Value.IsArchived.Should().BeTrue();
    }

    [Fact]
    public async Task ArchiveWallet_WhenWalletIsAlreadyArchived_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new WalletId(walletId));
        wallet.Value.Archive();

        await walletRepository.UpdateAsync(wallet.Value);

        var request = Requests.Wallets.ArchiveWallet(walletId);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task ArchiveWallet_WhenWalletDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Wallets.ArchiveWallet(Ulid.NewUlid());

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ArchiveWallet_WhenUserDoesNotOwnWallet_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        var userRepository = GetRequiredService<IUserRepository>();
        await userRepository.AddAsync(otherUser.Value);

        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var request = Requests.Wallets.ArchiveWallet(walletId);

        var client = CreateClient(otherUserAccessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ArchiveWallet_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Wallets.ArchiveWallet(walletId);
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}