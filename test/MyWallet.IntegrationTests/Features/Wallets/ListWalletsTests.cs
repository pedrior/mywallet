using System.Net.Http.Json;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Domain.Wallets.Repository;
using MyWallet.Domain.Wallets.ValueObjects;
using MyWallet.Features.Wallets;
using MyWallet.Shared.Contracts;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class ListWalletsTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private const int DefaultWalletsCount = 10;

    private string accessToken = null!;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        var walletRepository = GetRequiredService<IWalletRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        accessToken = CreateAccessToken(user.Value);

        for (var i = 0; i < DefaultWalletsCount; i++)
        {
            var wallet = Factories.Wallet.CreateDefault(
                id: WalletId.New(),
                userId: user.Value.Id);

            await walletRepository.AddAsync(wallet);
        }
    }

    [Fact]
    public async Task ListWallets_WhenUserOwnsWallets_ShouldReturnPaginatedWallets()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Wallets.ListWallets(page: 1, limit: 5);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var pageResponse = await response.Content
            .ReadFromJsonAsync<PageResponse<WalletSummaryResponse>>();

        pageResponse.Should().NotBeNull();
        pageResponse!.Page.Should().Be(1);
        pageResponse.Limit.Should().Be(5);
        pageResponse.Total.Should().Be(DefaultWalletsCount);
    }

    [Fact]
    public async Task ListWallets_WhenUserDoesNotHaveWallets_ShouldReturnEmptyPaginatedWallets()
    {
        // Arrange
        var userRepository = GetRequiredService<IUserRepository>();

        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherAccessToken = CreateAccessToken(otherUser.Value);

        var client = CreateClient(otherAccessToken);
        var request = Requests.Wallets.ListWallets(page: 1, limit: 5);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var pageResponse = await response.Content
            .ReadFromJsonAsync<PageResponse<WalletSummaryResponse>>();

        pageResponse.Should().NotBeNull();
        pageResponse!.Page.Should().Be(1);
        pageResponse.Limit.Should().Be(5);
        pageResponse.Total.Should().Be(0);
    }

    [Fact]
    public async Task ListWallets_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();

        var request = Requests.Wallets.ListWallets(page: 1, limit: 5);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}