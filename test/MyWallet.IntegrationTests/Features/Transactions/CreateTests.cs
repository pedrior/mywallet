using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.IntegrationTests.Features.Transactions;

public sealed class CreateTests(TestApplicationFactory app) : IntegrationTest(app)
{
    private IUserRepository userRepository = null!;
    private IWalletRepository walletRepository = null!;
    private ICategoryRepository categoryRepository = null!;
    private ITransactionRepository transactionRepository = null!;

    private string accessToken = null!;

    private UserId userId = null!;
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
    }

    private HttpRequestMessage GetRequest() => Requests.Transactions.CreateTransaction(
        walletId: walletId.Value,
        categoryId: categoryId.Value,
        type: Constants.Transaction.Type.Name,
        name: Constants.Transaction.Name.Value,
        amount: Constants.Transaction.Amount.Value,
        currency: Constants.Transaction.Currency.Name,
        date: Constants.Transaction.Date);

    [Fact]
    public async Task CreateTransaction_WhenRequestIsValid_ShouldCreateTransaction()
    {
        // Arrange
        var request = GetRequest();
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var transactionId = response.Headers.Location!
            .ToString()
            .Split('/')
            .Last();

        var transaction = await transactionRepository.GetAsync(new(Ulid.Parse(transactionId)));
        transaction.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTransaction_WhenWalletIsArchived_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var wallet = await walletRepository.GetAsync(walletId);
        wallet!.Archive();

        await walletRepository.UpdateAsync(wallet);

        var request = GetRequest();
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateTransaction_WhenTypeAndCategoryTypeDoNotMatch_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var otherCategory = Factories.Category.CreateDefault(
            id: CategoryId.New(),
            userId: userId,
            type: CategoryType.Income);

        await categoryRepository.AddAsync(otherCategory);

        categoryId = otherCategory.Id;

        var request = GetRequest();

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task
        CreateTransaction_WhenWalletCurrencyAndTransactionCurrencyDoNotMatch_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var otherWallet = Factories.Wallet.CreateDefault(
            id: WalletId.New(),
            userId: userId,
            currency: Currency.EUR);

        await walletRepository.AddAsync(otherWallet);

        walletId = otherWallet.Id;

        var request = GetRequest();

        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateTransaction_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = GetRequest();
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTransaction_WhenWalletDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await walletRepository.DeleteAsync(walletId);

        var request = GetRequest();
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTransaction_WhenCategoryDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await categoryRepository.DeleteAsync(categoryId);

        var request = GetRequest();
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTransaction_WhenUserDoesNotOwnWallet_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherWallet = Factories.Wallet.CreateDefault(
            id: WalletId.New(),
            userId: otherUser.Value.Id);

        await walletRepository.AddAsync(otherWallet);

        walletId = otherWallet.Id;

        var request = GetRequest();
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateTransaction_WhenUserDoesNotOwnCategory_ShouldReturnForbidden()
    {
        // Arrange
        var otherUser = await Factories.User.CreateDefaultWithServiceProvider(
            Services,
            id: UserId.New(),
            email: Constants.User.Email2);

        await userRepository.AddAsync(otherUser.Value);

        var otherCategory = Factories.Category.CreateDefault(
            id: CategoryId.New(),
            userId: otherUser.Value.Id);

        await categoryRepository.AddAsync(otherCategory);

        categoryId = otherCategory.Id;

        var request = GetRequest();
        var client = CreateClient(accessToken);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task CreateUserAsync()
    {
        var user = await Factories.User.CreateDefaultWithServiceProvider(Services);
        await userRepository.AddAsync(user.Value);

        userId = user.Value.Id;
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
}