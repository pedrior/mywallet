using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class EditWalletTests(TestApplicationFactory app) : IntegrationTest(app)
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
    public async Task EditWallet_WhenRequestIsValid_ShouldChangeEditWallet()
    {
        // Arrange
        var request = Requests.Wallets.EditWallet(
            id: walletId,
            name: Constants.Wallet.Name2.Value,
            color: Constants.Wallet.Color2.Value,
            currency: Constants.Wallet.Currency2.Name);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new(walletId));
        
        wallet.IsError.Should().BeFalse();
        wallet.Value.Should().BeEquivalentTo(new
        {
            Name = Constants.Wallet.Name2,
            Color = Constants.Wallet.Color2,
            Currency = Constants.Wallet.Currency2
        });
    }

    [Fact]
    public async Task EditWallet_WhenWalletDoesNotExist_ShouldReturnNotFound()
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
    public async Task EditWallet_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
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
    public async Task EditWallet_WhenUserDoesNotOwnWallet_ShouldReturnForbidden()
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