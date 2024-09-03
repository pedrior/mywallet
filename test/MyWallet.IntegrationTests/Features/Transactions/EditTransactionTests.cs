using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.IntegrationTests.Features.Transactions;

public sealed class EditTransactionTests(TestApplicationFactory app) : IntegrationTest(app)
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
    public async Task EditTransaction_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var client = CreateClient(accessToken);

        // We don't to change anything, so we just need to send the request
        var request = Requests.Transactions.EditTransaction(
            transactionId: transactionId.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task EditTransaction_WhenRequestIsValid_ShouldUpdateTransaction()
    {
        // Arrange
        var wallet2 = Factories.Wallet.CreateDefault2();
        var category2 = Factories.Category.CreateDefault2();

        await walletRepository.AddAsync(wallet2);
        await categoryRepository.AddAsync(category2);

        var client = CreateClient(accessToken);
        var request = Requests.Transactions.EditTransaction(
            transactionId.Value,
            walletId: Constants.Wallet.Id2.Value,
            categoryId: Constants.Category.Id2.Value,
            name: Constants.Transaction.Name2.Value,
            amount: Constants.Transaction.Amount2.Value,
            currency: Constants.Transaction.Currency2.Name,
            date: Constants.Transaction.Date2);

        // Act
        await client.SendAsync(request);

        // Assert

        var transaction = await transactionRepository.GetAsync(transactionId);
        
        transaction.IsError.Should().BeFalse();
        transaction.Value.Should().BeEquivalentTo(new
        {
            WalletId = Constants.Wallet.Id2,
            CategoryId = Constants.Category.Id2,
            Name = Constants.Transaction.Name2,
            Amount = Constants.Transaction.Amount2,
            Currency = Constants.Transaction.Currency2,
            Date = Constants.Transaction.Date2
        });
    }

    [Fact]
    public async Task EditTransaction_WhenTransactionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.EditTransaction(
            transactionId: TransactionId.New().Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditTransaction_WHenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateClient();
        var request = Requests.Transactions.EditTransaction(transactionId.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task EditTransaction_WhenTransactionIsOwnedByAnotherUser_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var client = CreateClient(otherUserAccessToken);
        var request = Requests.Transactions.EditTransaction(transactionId.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task EditTransaction_WhenChangingWalletToNonExistingWallet_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.EditTransaction(
            transactionId.Value,
            walletId: WalletId.New().Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditTransaction_WhenChangingCategoryToNonExistingCategory_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient(accessToken);
        var request = Requests.Transactions.EditTransaction(
            transactionId.Value,
            categoryId: CategoryId.New().Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditTransaction_WhenChangingWalletToWalletOfAnotherUser_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherUserWallet = Factories.Wallet.CreateDefault2();
        await walletRepository.AddAsync(otherUserWallet);

        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var client = CreateClient(otherUserAccessToken);
        var request = Requests.Transactions.EditTransaction(
            transactionId.Value,
            walletId: otherUserWallet.Id.Value);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task EditTransaction_WhenChangingCategoryToCategoryOfAnotherUser_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherUserCategory = Factories.Category.CreateDefault2();
        await categoryRepository.AddAsync(otherUserCategory);

        var otherUserAccessToken = CreateAccessToken(otherUser.Value);

        var client = CreateClient(otherUserAccessToken);
        var request = Requests.Transactions.EditTransaction(
            transactionId.Value,
            categoryId: otherUserCategory.Id.Value);

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