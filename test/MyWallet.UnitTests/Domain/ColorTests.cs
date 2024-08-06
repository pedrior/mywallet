using MyWallet.Domain;

namespace MyWallet.UnitTests.Domain;

[TestSubject(typeof(Color))]
public sealed class ColorTests
{
    public static readonly TheoryData<string?> EmptyColorValues = new([
        null,
        string.Empty,
        "  "
    ]);

    public static readonly TheoryData<string> ValidColorValues = new([
        "#FFF",
        "#202020",
        "#f9eb12"
    ]);

    public static readonly TheoryData<string> InvalidColorValues = new([
        "#ff",
        "987",
        "202020",
        "####",
        "#L444",
        "U212647",
        "8#FF",
        "8FF00000",
        "#EB546D1"
    ]);

    [Theory, MemberData(nameof(EmptyColorValues))]
    public void Create_WhenValueIsEmpty_ShouldReturnIsEmptyError(string? value)
    {
        // Arrange
        // Act
        var result = Color.Create(value!);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Color.IsEmpty);
    }

    [Theory, MemberData(nameof(InvalidColorValues))]
    public void Create_WhenValueIsInvalid_ShouldReturnInvalidError(string value)
    {
        // Arrange
        // Act
        var result = Color.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Color.Invalid);
    }

    [Theory, MemberData(nameof(ValidColorValues))]
    public void Create_WhenValueIsValid_ShouldReturnColor(string value)
    {
        // Arrange
        // Act
        var result = Color.Create(value);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(value.ToUpperInvariant());
    }
}