using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.IntegrationTests.Features.Transactions;

public sealed class DeleteTransactionTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private IUserRepository userRepository = null!;
    private IWalletRepository walletRepository = null!;
    private ICategoryRepository categoryRepository = null!;
    private ITransactionRepository transactionRepository = null!;

    private string accessToken = null!;

    private WalletId walletId = null!;
    private CategoryId categoryId = null!;
    private TransactionId transactionId = null!;

    public override async Task InitializeAsync()
    {
        userRepository = GetRequiredService<IUserRepository>();
        walletRepository = GetRequiredService<IWalletRepository>();
        categoryRepository = GetRequiredService<ICategoryRepository>();
        transactionRepository = GetRequiredService<ITransactionRepository>();

        await CreateUserAsync();
        await CreateWalletAsync();
        await CreateCategoryAsync();
        await CreateTransactionAsync();
    }

    [Fact]
    public async Task DeleteTransaction_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.DeleteTransaction(
            transactionId.Value);
        
        // Act
        var response = await client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteTransaction_WhenRequestIsValid_ShouldDeleteTransaction()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.DeleteTransaction(
            transactionId.Value);
        
        // Act
        await client.SendAsync(request);
        
        // Assert
        var transaction = await transactionRepository.GetAsync(transactionId);
        
        transaction.IsError.Should().BeTrue();
        transaction.FirstError.Should().Be(TransactionErrors.NotFound);
    }
    
    [Fact]
    public async Task DeleteTransaction_WhenTransactionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.DeleteTransaction(
            transactionId: TransactionId.New().Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTransaction_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();
        var request = Requests.Transactions.DeleteTransaction(transactionId.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteTransaction_WhenTransactionIsOwnedByAnotherUser_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var client = CreateClient(otherUserAccessToken);
        var request = Requests.Transactions.DeleteTransaction(transactionId.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task CreateUserAsync()
    {
        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        accessToken = CreateAccessToken(user.Value);
    }

    private async Task CreateWalletAsync()
    {
        var wallet = Factories.Wallet.CreateDefault();
        await walletRepository.AddAsync(wallet);

        walletId = wallet.Id;
    }

    private async Task CreateCategoryAsync()
    {
        var category = Factories.Category.CreateDefault();
        await categoryRepository.AddAsync(category);

        categoryId = category.Id;
    }

    private async Task CreateTransactionAsync()
    {
        var transaction = Factories.Transaction.CreateDefault(
            walletId: walletId,
            categoryId: categoryId);

        await transactionRepository.AddAsync(transaction.Value);

        transactionId = transaction.Value.Id;
    }
}