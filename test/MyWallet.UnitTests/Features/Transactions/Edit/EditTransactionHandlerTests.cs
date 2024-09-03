using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;
using MyWallet.Features.Transactions.Edit;
using TransactionErrors = MyWallet.Features.Transactions.Shared.TransactionErrors;

namespace MyWallet.UnitTests.Features.Transactions.Edit;

public sealed class EditTransactionHandlerTests
{
    private readonly ITransactionRepository transactionRepository = Substitute.For<ITransactionRepository>();
    private readonly IWalletRepository walletRepository = Substitute.For<IWalletRepository>();
    private readonly ICategoryRepository categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly EditTransactionHandler sut;

    private static readonly EditTransactionCommand Command = new()
    {
        TransactionId = Constants.Transaction.Id.Value,
        UserId = Constants.User.Id.Value,
        WalletId = Constants.Wallet.Id2.Value,
        CategoryId = Constants.Category.Id2.Value,
        Name = Constants.Transaction.Name2.Value,
        Amount = Constants.Transaction.Amount2.Value,
        Currency = Constants.Transaction.Currency2.Name,
        Date = Constants.Transaction.Date2
    };

    private readonly Transaction transaction = Factories.Transaction.CreateDefault().Value;
    private readonly Wallet wallet = Factories.Wallet.CreateDefault();
    private readonly Category category = Factories.Category.CreateDefault();

    public EditTransactionHandlerTests()
    {
        sut = new EditTransactionHandler(transactionRepository, walletRepository, categoryRepository);

        transactionRepository.GetAsync(transaction.Id, Arg.Any<CancellationToken>())
            .Returns(transaction);

        walletRepository.GetAsync(
                Arg.Is<WalletId>(id => id.Value == Command.WalletId),
                Arg.Any<CancellationToken>())
            .Returns(wallet);

        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(id => id.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .Returns(category);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnUpdated()
    {
        // Arrange
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Updated);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldUpdateTransaction()
    {
        // Arrange
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await transactionRepository
            .Received(1)
            .UpdateAsync(transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldChangeTransaction()
    {
        // Arrange
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        transaction.WalletId.Should().Be(wallet.Id);
        transaction.CategoryId.Should().Be(category.Id);
        transaction.Name.Value.Should().Be(Command.Name);
        transaction.Amount.Value.Should().Be(Command.Amount);
        transaction.Currency.Name.Should().Be(Command.Currency);
        transaction.Date.Should().Be(Command.Date);
    }

    [Fact]
    public async Task Handle_WhenTransactionDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        transactionRepository.GetAsync(transaction.Id, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenNameIsNull_ShouldNotChangeName()
    {
        // Arrange
        var command = Command with { Name = null };

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        transaction.Name.Value.Should().Be(Constants.Transaction.Name.Value);
    }

    [Fact]
    public async Task Handle_WhenAmountIsNull_ShouldNotChangeAmount()
    {
        // Arrange
        var command = Command with { Amount = null };

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        transaction.Amount.Value.Should().Be(Constants.Transaction.Amount.Value);
    }

    [Fact]
    public async Task Handle_WhenCurrencyIsNull_ShouldNotChangeCurrency()
    {
        // Arrange
        var command = Command with { Currency = null };

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        transaction.Currency.Name.Should().Be(Constants.Transaction.Currency.Name);
    }

    [Fact]
    public async Task Handle_WhenDateIsNull_ShouldNotChangeDate()
    {
        // Arrange
        var command = Command with { Date = null };

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        transaction.Date.Should().Be(Constants.Transaction.Date);
    }

    [Fact]
    public async Task Handle_WhenWalletIdIsNull_ShouldNotChangeWallet()
    {
        // Arrange
        var command = Command with { WalletId = null };

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        transaction.WalletId.Should().Be(Constants.Wallet.Id);

        await walletRepository
            .DidNotReceive()
            .GetAsync(
                Arg.Is<WalletId>(id => id.Value == Command.WalletId),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryIdIsNull_ShouldNotChangeCategory()
    {
        // Arrange
        var command = Command with { CategoryId = null };

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        transaction.CategoryId.Should().Be(Constants.Category.Id);

        await categoryRepository
            .DidNotReceive()
            .GetAsync(
                Arg.Is<CategoryId>(id => id.Value == Command.CategoryId),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChangingWalletAndWalletDoesNotExist_ShouldReturnNotFoundError()
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
    public async Task Handle_WhenChangingCategoryAndCategoryDoesNotExist_ShouldReturnNotFoundError()
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

    [Fact]
    public async Task Handle_WhenChangingWalletAndWalletDoesNotExist_ShouldNotUpdateTransaction()
    {
        // Arrange
        walletRepository.GetAsync(
                Arg.Is<WalletId>(id => id.Value == Command.WalletId),
                Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await transactionRepository
            .DidNotReceive()
            .UpdateAsync(transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChangingCategoryAndCategoryDoesNotExist_ShouldNotUpdateTransaction()
    {
        // Arrange
        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(id => id.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await transactionRepository
            .DidNotReceive()
            .UpdateAsync(transaction, Arg.Any<CancellationToken>());
    }
}