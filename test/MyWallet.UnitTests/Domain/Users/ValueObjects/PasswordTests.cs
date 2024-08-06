using MyWallet.Domain.Users.ValueObjects;

namespace MyWallet.UnitTests.Domain.Users.ValueObjects;

[TestSubject(typeof(Password))]
public sealed class PasswordTests
{
    public static readonly TheoryData<string?> EmptyPasswordValues = new([
        null,
        string.Empty,
        "  "
    ]);

    [Theory, MemberData(nameof(EmptyPasswordValues))]
    public void Create_WhenValueIsEmpty_ShouldReturnIsEmptyError(string? value)
    {
        // Arrange
        // Act
        var result = Password.Create(value!);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Password.IsEmpty);
    }

    [Fact]
    public void Create_WhenValueIsTooShort_ShouldReturnTooShortError()
    {
        // Arrange
        const string value = "12345";

        // Act
        var result = Password.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Password.TooShort);
    }

    [Fact]
    public void Create_WhenValueIsTooLong_ShouldReturnTooLongError()
    {
        // Arrange
        var value = new string('a', 31);

        // Act
        var result = Password.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Password.TooLong);
    }

    [Fact]
    public void Create_WhenValueHasNoUppercaseLetters_ShouldReturnNoUppercaseLettersError()
    {
        // Arrange
        const string value = "johndoe123";

        // Act
        var result = Password.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Password.NoUppercaseLetters);
    }

    [Fact]
    public void Create_WhenValueHasNoLowercaseLetters_ShouldReturnNoLowercaseLettersError()
    {
        // Arrange
        const string value = "JOHNDOE123";

        // Act
        var result = Password.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Password.NoLowercaseLetters);
    }

    [Fact]
    public void Create_WhenValueHasNoDigits_ShouldReturnNoDigitsError()
    {
        // Arrange
        const string value = "JohnDoe";

        // Act
        var result = Password.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Password.NoDigits);
    }

    [Fact]
    public void Create_WhenValueIsValid_ShouldReturnPassword()
    {
        // Arrange
        const string value = "JohnDoe123";

        // Act
        var result = Password.Create(value);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(value);
    }
}