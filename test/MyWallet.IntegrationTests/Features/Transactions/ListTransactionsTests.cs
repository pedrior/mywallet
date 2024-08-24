using System.Net.Http.Json;
using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;
using MyWallet.Features.Transactions.List;

namespace MyWallet.IntegrationTests.Features.Transactions;

public sealed class ListTransactionsTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private static readonly (TransactionType type, decimal amount, DateOnly date)[] Transactions =
    [
        (TransactionType.Expense, 97.5m, DateOnly.Parse("2024-07-20")),
        (TransactionType.Expense, 180m, DateOnly.Parse("2024-07-20")),
        (TransactionType.Expense, 257.32m, DateOnly.Parse("2024-07-20")),
        (TransactionType.Income, 990m, DateOnly.Parse("2024-07-20")),
        (TransactionType.Income, 150m, DateOnly.Parse("2024-07-22")),
        (TransactionType.Expense, 150m, DateOnly.Parse("2024-07-22")),
        (TransactionType.Expense, 498m, DateOnly.Parse("2024-07-22")),
        (TransactionType.Income, 80.98m, DateOnly.Parse("2024-07-22")),
        (TransactionType.Expense, 78m, DateOnly.Parse("2024-07-23")),
        (TransactionType.Income, 888.75m, DateOnly.Parse("2024-07-24")),
        (TransactionType.Income, 80.69m, DateOnly.Parse("2024-07-24")),
        (TransactionType.Expense, 700m, DateOnly.Parse("2024-07-25"))
    ];

    private IUserRepository userRepository = null!;
    private IWalletRepository walletRepository = null!;
    private ICategoryRepository categoryRepository = null!;
    private ITransactionRepository transactionRepository = null!;

    private string accessToken = null!;

    private WalletId walletId = null!;
    private CategoryId categoryId = null!;

    public override async Task InitializeAsync()
    {
        userRepository = GetRequiredService<IUserRepository>();
        walletRepository = GetRequiredService<IWalletRepository>();
        categoryRepository = GetRequiredService<ICategoryRepository>();
        transactionRepository = GetRequiredService<ITransactionRepository>();

        await CreateUserAsync();
        await CreateWalletAsync();
        await CreateCategoryAsync();
        await CreateTransactionsAsync();
    }

    [Fact]
    public async Task ListTransactions_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.ListTransactions(
            walletId: walletId.Value,
            from: Transactions.Min(t => t.date),
            limit: 10);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var listTransactionsResponse = await response.Content
            .ReadFromJsonAsync<ListTransactionsResponse>();

        listTransactionsResponse.Should().NotBeNull();

        listTransactionsResponse!.Items.Should().HaveCount(10);
        listTransactionsResponse.Total.Should().Be(Transactions.Length);
        listTransactionsResponse.Page.Should().Be(1);
        listTransactionsResponse.Limit.Should().Be(10);
    }

    [Fact]
    public async Task ListTransactions_WhenUserDoesNotOwnWallet_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var client = CreateClient(otherUserAccessToken);
        var request = Requests.Transactions.ListTransactions(
            walletId: walletId.Value,
            from: DateOnly.MinValue);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ListTransactions_WhenUserDoesNotHaveTransactions_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.ListTransactions(
            walletId: walletId.Value,
            from: DateOnly.MaxValue);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var listTransactionsResponse = await response.Content
            .ReadFromJsonAsync<ListTransactionsResponse>();

        listTransactionsResponse.Should().NotBeNull();

        listTransactionsResponse!.Items.Should().BeEmpty();
        listTransactionsResponse.Total.Should().Be(0);
        listTransactionsResponse.Page.Should().Be(1);
        listTransactionsResponse.Limit.Should().Be(10);
    }

    [Fact]
    public async Task ListTransactions_WhenWalletDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.ListTransactions(
            walletId: WalletId.New().Value,
            from: DateOnly.MinValue);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ListTransaction_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();
        var request = Requests.Transactions.ListTransactions(
            walletId: walletId.Value,
            from: DateOnly.MinValue);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ListTransaction_WhenUserDoesNotOwnTransaction_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var client = CreateClient(otherUserAccessToken);
        var request = Requests.Transactions.ListTransactions(
            walletId: walletId.Value,
            from: DateOnly.MinValue);

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

    private async Task CreateTransactionsAsync()
    {
        foreach (var (type, amount, date) in Transactions)
        {
            var transaction = Factories.Transaction.CreateDefault(
                id: TransactionId.New(),
                walletId: walletId,
                categoryId: categoryId,
                type: type,
                amount: Amount.Create(amount).Value,
                date: date);

            await transactionRepository.AddAsync(transaction.Value);
        }
    }
}