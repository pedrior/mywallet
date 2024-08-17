using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;
using MyWallet.Features.Transactions.Create;

namespace MyWallet.UnitTests.Features.Transactions.Create;

public sealed class CreateTransactionHandlerTests
{
    private readonly ITransactionService transactionService = A.Fake<ITransactionService>();
    private readonly ITransactionRepository transactionRepository = A.Fake<ITransactionRepository>();
    private readonly IWalletRepository walletRepository = A.Fake<IWalletRepository>();
    private readonly ICategoryRepository categoryRepository = A.Fake<ICategoryRepository>();

    private readonly CreateTransactionHandler sut;

    private readonly Wallet wallet = Factories.Wallet.CreateDefault();
    private readonly Category category = Factories.Category.CreateDefault();
    private readonly Transaction transaction = Factories.Transaction.CreateDefault().Value;

    private static readonly CreateTransactionCommand Command = new()
    {
        WalletId = Constants.Wallet.Id.Value,
        CategoryId = Constants.Category.Id.Value,
        Type = Constants.Transaction.Type.Name,
        Name = Constants.Transaction.Name.Value,
        Amount = Constants.Transaction.Amount.Value,
        Currency = Constants.Transaction.Currency.Name,
        Date = Constants.Transaction.Date,
        UserId = Constants.User.Id.Value
    };

    public CreateTransactionHandlerTests()
    {
        sut = new CreateTransactionHandler(
            transactionService,
            transactionRepository,
            walletRepository,
            categoryRepository);

        A.CallTo(() => walletRepository.GetAsync(
                A<WalletId>.That.Matches(id => id.Value == Command.WalletId),
                A<CancellationToken>.Ignored))
            .Returns(wallet);

        A.CallTo(() => categoryRepository.GetAsync(
                A<CategoryId>.That.Matches(id => id.Value == Command.CategoryId),
                A<CancellationToken>.Ignored))
            .Returns(category);

        A.CallTo(() => transactionService.CreateTransaction(
                wallet,
                category,
                A<TransactionType>.That.Matches(type => type.Name == Command.Type),
                A<TransactionName>.That.Matches(name => name.Value == Command.Name),
                A<Amount>.That.Matches(amount => amount.Value == Command.Amount),
                A<Currency>.That.Matches(currency => currency.Name == Command.Currency),
                Command.Date))
            .Returns(transaction);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnTransactionId()
    {
        // Arrange
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(transaction.Id.Value);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldAddTransactionToRepository()
    {
        // Arrange
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => transactionRepository.AddAsync(transaction, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}