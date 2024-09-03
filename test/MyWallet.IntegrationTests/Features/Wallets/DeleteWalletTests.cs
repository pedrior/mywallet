using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Persistence;

namespace MyWallet.IntegrationTests.Features.Wallets;

public sealed class DeleteWalletTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private string accessToken = null!;
    private Ulid walletId;

    public override async Task InitializeAsync()
    {
        var userRepository = GetRequiredService<IUserRepository>();
        var walletRepository = GetRequiredService<IWalletRepository>();
        var categoryRepository = GetRequiredService<ICategoryRepository>();
        var transactionRepository = GetRequiredService<ITransactionRepository>();

        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        accessToken = CreateAccessToken(user.Value);

        var wallet = Factories.Wallet.CreateDefault(userId: user.Value.Id);
        await walletRepository.AddAsync(wallet);
        
        var category = Factories.Category.CreateDefault(userId: user.Value.Id);
        await categoryRepository.AddAsync(category);
        
        var transaction = Factories.Transaction.CreateDefault(
            walletId: wallet.Id,
            categoryId: category.Id);
        
        await transactionRepository.AddAsync(transaction.Value);

        walletId = wallet.Id.Value;
    }

    [Fact]
    public async Task DeleteWallet_WhenRequestIsValid_ShouldDeleteWallet()
    {
        // Arrange
        var request = Requests.Wallets.DeleteWallet(walletId);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var walletRepository = GetRequiredService<IWalletRepository>();
        var wallet = await walletRepository.GetAsync(new WalletId(walletId));

        wallet.IsError.Should().BeTrue();
        wallet.FirstError.Should().Be(WalletErrors.NotFound);
    }
    
    [Fact]
    public async Task DeleteWallet_WhenRequestIsValid_ShouldDeleteWalletTransactions()
    {
        // Arrange
        var request = Requests.Wallets.DeleteWallet(walletId);

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var db = GetRequiredService<IDbContext>();
        var anyWalletTransaction = await db.ExecuteScalarAsync<bool>(
            sql: """
                    SELECT 1
                    FROM transactions
                    WHERE wallet_id = @WalletId
                 """,
            param: new { WalletId = walletId });

        anyWalletTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteWallet_WhenWalletDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Wallets.DeleteWallet(Ulid.NewUlid());

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

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

        var request = Requests.Wallets.DeleteWallet(walletId);

        var client = CreateClient(otherUserAccessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteWallet_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Wallets.DeleteWallet(walletId);
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}