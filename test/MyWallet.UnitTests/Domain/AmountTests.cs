using MyWallet.Domain;

namespace MyWallet.UnitTests.Domain;

[TestSubject(typeof(Amount))]
public sealed class AmountTests
{
    [Theory, InlineData(0), InlineData(1.30)]
    public void Create_WhenValueIsPositive_ShouldReturnAmount(decimal value)
    {
        // Act
        var result = Amount.Create(value);
        
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WhenValueIsNegative_ShouldReturnIsNegativeError()
    {
        // Arrange
        const decimal value = -1.30m;
        
        // Act
        var result = Amount.Create(value);
        
        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Amount.IsNegative);
    }
}