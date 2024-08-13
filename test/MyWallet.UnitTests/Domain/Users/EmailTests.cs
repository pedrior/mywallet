using MyWallet.Domain.Users;

namespace MyWallet.UnitTests.Domain.Users;

[TestSubject(typeof(Email))]
public sealed class EmailTests
{
    public static readonly TheoryData<string?> EmptyEmailValues = new([
        null,
        string.Empty,
        "  "
    ]);

    public static readonly TheoryData<string> ValidEmailValues = new([
        "john@doe.com",
        "john.doe@jane.com",
        "john123@doe.co",
        "john@doe.co.uk"
    ]);

    public static readonly TheoryData<string> InvalidEmailValues = new([
        "johndoe.com",
        "john@doe",
        "john@doe.",
        "john@doe@com",
        "john@doe..com",
        "john@doe-.com",
        "john@doe.-com"
    ]);

    [Theory, MemberData(nameof(EmptyEmailValues))]
    public void Create_WhenValueIsEmpty_ShouldReturnIsEmptyError(string? value)
    {
        // Arrange
        // Act
        var result = Email.Create(value!);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Email.IsEmpty);
    }

    [Fact]
    public void Create_WhenValueIsTooLong_ShouldReturnTooLongError()
    {
        // Arrange
        var value = $"{new string('a', 250)}@a.co";

        // Act
        var result = Email.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Email.TooLong);
    }

    [Theory]
    [MemberData(nameof(ValidEmailValues))]
    public void Create_WhenValueIsValid_ShouldReturnEmail(string value)
    {
        // Arrange
        // Act
        var result = Email.Create(value);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(value.ToLowerInvariant());
    }

    [Theory]
    [MemberData(nameof(InvalidEmailValues))]
    public void Create_WhenValueIsInvalid_ShouldReturnInvalidError(string value)
    {
        // Arrange
        // Act
        var result = Email.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Email.Invalid);
    }
}