using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Wallets.Repository;
using MyWallet.Domain.Wallets.ValueObjects;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class CreateWalletTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private string accessToken = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);

        await userRepository.AddAsync(user.Value);

        accessToken = CreateAccessToken(user.Value);
    }

    [Fact]
    public async Task CreateWallet_WhenRequestIsValid_ShouldCreateWallet()
    {
        // Arrange
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(Requests.Wallets.CreateWallet());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Get the category id from the response location header
        var walletId = response.Headers.Location!
            .ToString()
            .Split('/')
            .Last();

        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new WalletId(Ulid.Parse(walletId)));

        wallet.Should().NotBeNull();
        wallet!.Name.Should().Be(Constants.Wallet.Name);
        wallet.Color.Should().Be(Constants.Wallet.Color);
    }
    
    [Fact]
    public async Task CreateWallet_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(Requests.Wallets.CreateWallet());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}