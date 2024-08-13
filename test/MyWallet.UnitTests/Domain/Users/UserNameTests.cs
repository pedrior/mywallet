using MyWallet.Domain.Users;

namespace MyWallet.UnitTests.Domain.Users;

[TestSubject(typeof(UserName))]
public sealed class UserNameTests
{
    public static readonly TheoryData<string?> EmptyUserNameValues = new([
        null,
        string.Empty,
        "  "
    ]);

    public static readonly TheoryData<string> InvalidUserNameValues = new([
        ". John",
        "'John Doe",
        "John Doe.",
        "John'",
        "John Doe 2",
        "O'Neil;",
        "  John Doe",
        "John Doe  ",
    ]);

    public static readonly TheoryData<string> ValidUserNameValues = new([
        "John",
        "John Doe",
        "John O'Neil",
        "D. John",
        "João",
        "Mr. John's",
        "Gusmão N. Carlos"
    ]);

    [Theory, MemberData(nameof(EmptyUserNameValues))]
    public void Create_WhenValueIsEmpty_ShouldReturnIsEmptyError(string? value)
    {
        // Arrange
        // Act
        var result = UserName.Create(value!);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(UserName.IsEmpty);
    }

    [Fact]
    public void Create_WhenValueIsTooShort_ShouldReturnTooShortError()
    {
        // Arrange
        const string value = "a";

        // Act
        var result = UserName.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(UserName.TooShort);
    }

    [Fact]
    public void Create_WhenValueIsTooLong_ShouldReturnTooLongError()
    {
        // Arrange
        var value = new string('a', 51);

        // Act
        var result = UserName.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(UserName.TooLong);
    }

    [Theory, MemberData(nameof(InvalidUserNameValues))]
    public void Create_WhenValueIsMalformed_ShouldReturnMalformedError(string value)
    {
        // Arrange
        // Act
        var result = UserName.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(UserName.Malformed);
    }

    [Theory, MemberData(nameof(ValidUserNameValues))]
    public void Create_WhenValueIsValid_ShouldReturnUserName(string value)
    {
        // Arrange
        // Act
        var result = UserName.Create(value);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(value);
    }
}