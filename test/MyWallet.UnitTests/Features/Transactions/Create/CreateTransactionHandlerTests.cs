using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;
using MyWallet.Features.Transactions.Create;
using TransactionErrors = MyWallet.Features.Transactions.Shared.TransactionErrors;

namespace MyWallet.UnitTests.Features.Transactions.Create;

public sealed class CreateTransactionHandlerTests
{
    private readonly ITransactionService transactionService = new TransactionService();
    private readonly ITransactionRepository transactionRepository = Substitute.For<ITransactionRepository>();
    private readonly IWalletRepository walletRepository = Substitute.For<IWalletRepository>();
    private readonly ICategoryRepository categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly CreateTransactionHandler sut;
    
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
        
        walletRepository.GetAsync(
                Arg.Is<WalletId>(id => id.Value == Command.WalletId),
                Arg.Any<CancellationToken>())
            .Returns(Factories.Wallet.CreateDefault());

        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(id => id.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .Returns(Factories.Category.CreateDefault());
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnTransactionId()
    {
        // Arrange
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBe(Ulid.Empty);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldAddTransactionToRepository()
    {
        // Arrange
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await transactionRepository
            .Received(1)
            .AddAsync(
                Arg.Is<Transaction>(t =>
                    t.WalletId.Value == Command.WalletId &&
                    t.CategoryId.Value == Command.CategoryId &&
                    t.Type.Name == Command.Type &&
                    t.Name.Value == Command.Name &&
                    t.Amount.Value == Command.Amount &&
                    t.Currency.Name == Command.Currency &&
                    t.Date == Command.Date),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenWalletDoesNotExist_ShouldReturnWalletNotFound()
    {
        // Arrange
        walletRepository.GetAsync(
                Arg.Is<WalletId>(id => id.Value == Command.WalletId),
                Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.WalletNotFound);
    }
    
    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldReturnCategoryNotFound()
    {
        // Arrange
        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(id => id.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.CategoryNotFound);
    }
}