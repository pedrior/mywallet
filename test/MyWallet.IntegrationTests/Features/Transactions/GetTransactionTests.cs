using System.Net.Http.Json;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;
using MyWallet.Features.Transactions.Get;

namespace MyWallet.IntegrationTests.Features.Transactions;

public sealed class GetTransactionTests(TestApplicationFactory app) : IntegrationTest(app)
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

    private async Task CreateUserAsync()
    {
        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        accessToken = CreateAccessToken(user.Value);
    }

    [Fact]
    public async Task GetTransaction_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.GetTransaction(transactionId.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var transactionResponse = await response.Content
            .ReadFromJsonAsync<GetTransactionResponse>();

        transactionResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTransaction_WhenTransactionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.GetTransaction(TransactionId.New().Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTransaction_WHenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();
        var request = Requests.Transactions.GetTransaction(transactionId.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTransaction_WhenUserDoesNotOwnTransaction_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var client = CreateClient(otherUserAccessToken);
        var request = Requests.Transactions.GetTransaction(transactionId.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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