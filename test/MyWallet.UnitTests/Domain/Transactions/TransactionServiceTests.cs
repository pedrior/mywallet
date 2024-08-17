using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;

namespace MyWallet.UnitTests.Domain.Transactions;

[TestSubject(typeof(TransactionService))]
public sealed class TransactionServiceTests
{
    private readonly TransactionService transactionService = new();

    private readonly Wallet wallet = Factories.Wallet.CreateDefault(
        currency: Currency.BRL);

    private readonly Category category = Factories.Category.CreateDefault(
        type: CategoryType.Expense);
    
    [Fact]
    public void CreateTransaction_WhenCalled_ShouldReturnTransaction()
    {
        // Arrange
        // Act
        var result = transactionService.CreateTransaction(
            wallet,
            category,
            TransactionType.Expense,
            Constants.Transaction.Name,
            Constants.Transaction.Amount,
            Constants.Transaction.Currency,
            Constants.Transaction.Date);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.WalletId.Should().Be(wallet.Id);
        result.Value.CategoryId.Should().Be(category.Id);
        result.Value.Type.Should().Be(TransactionType.Expense);
        result.Value.Name.Should().Be(Constants.Transaction.Name);
        result.Value.Amount.Should().Be(Constants.Transaction.Amount);
        result.Value.Currency.Should().Be(Constants.Transaction.Currency);
        result.Value.Date.Should().Be(Constants.Transaction.Date);
    }

    [Fact]
    public void CreateTransaction_WhenWalletIsArchived_ShouldReturnWalletIsArchivedError()
    {
        // Arrange
        wallet.Archive();

        // Act
        var result = transactionService.CreateTransaction(
            wallet,
            category,
            TransactionType.Expense,
            Constants.Transaction.Name,
            Constants.Transaction.Amount,
            Constants.Transaction.Currency,
            Constants.Transaction.Date);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.WalletIsArchived);
    }

    [Fact]
    public void CreateTransaction_WhenCategoryTypeMismatch_ShouldReturnCategoryTypeMismatchError()
    {
        // Arrange
        // Act
        var result = transactionService.CreateTransaction(
            wallet,
            category,
            TransactionType.Income,
            Constants.Transaction.Name,
            Constants.Transaction.Amount,
            Constants.Transaction.Currency,
            Constants.Transaction.Date);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.CategoryTypeMismatch);
    }

    [Fact]
    public void CreateTransaction_WhenCurrencyMismatch_ShouldReturnCurrencyMismatchError()
    {
        // Arrange
        // Act
        var result = transactionService.CreateTransaction(
            wallet,
            category,
            TransactionType.Expense,
            Constants.Transaction.Name,
            Constants.Transaction.Amount,
            Currency.USD,
            Constants.Transaction.Date);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.CurrencyMismatch);
    }
}