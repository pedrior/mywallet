using MyWallet.Domain.Transactions;

namespace MyWallet.UnitTests.Domain.Transactions;

[TestSubject(typeof(TransactionName))]
public sealed class TransactionNameTests
{
    [Fact]
    public void Create_WhenValueIsValid_ReturnsTransactionName()
    {
        // Arrange
        const string value = "Dinner with friends";

        // Act
        var result = TransactionName.Create(value);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(value);
    }

    [Theory, InlineData(null), InlineData(""), InlineData(" ")]
    public void Create_WhenValueIsNullOrWhitespace_ReturnsIsEmptyError(string? value)
    {
        // Act
        var result = TransactionName.Create(value!);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionName.IsEmpty);
    }


    [Fact]
    public void Create_WhenValueIsLongerThanMaxLength_ReturnsTooLongError()
    {
        // Arrange
        var value = new string('a', TransactionName.MaxLength + 1);

        // Act
        var result = TransactionName.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionName.TooLong);
    }
}