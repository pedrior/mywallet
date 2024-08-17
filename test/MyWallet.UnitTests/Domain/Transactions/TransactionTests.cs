using MyWallet.Domain;
using MyWallet.Domain.Transactions;

namespace MyWallet.UnitTests.Domain.Transactions;

[TestSubject(typeof(Transaction))]
public sealed class TransactionTests
{
    [Fact]
    public void Create_WhenAmountIsZero_ShouldReturnAmountIsZeroError()
    {
        // Arrange
        var amount = Amount.Zero;

        // Act
        var result = Factories.Transaction.CreateDefault(amount: amount);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.AmountIsZero);
    }
}