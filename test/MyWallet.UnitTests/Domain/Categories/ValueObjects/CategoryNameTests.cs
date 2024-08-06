using MyWallet.Domain.Categories.ValueObjects;

namespace MyWallet.UnitTests.Domain.Categories.ValueObjects;

[TestSubject(typeof(CategoryName))]
public sealed class CategoryNameTests
{
    public static readonly TheoryData<string?> EmptyCategoryNameValues = new([
        null,
        string.Empty,
        "  "
    ]);

    public static readonly TheoryData<string> ValidCategoryNameValues = new([
        "Shopping",
        "PS4 Games",
        "Day #20",
        "Others $$$",
        "TO SAVE 20%"
    ]);

    [Theory, MemberData(nameof(EmptyCategoryNameValues))]
    public void Create_WhenValueIsEmpty_ShouldReturnIsEmptyError(string? value)
    {
        // Arrange
        // Act
        var result = CategoryName.Create(value!);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CategoryName.IsEmpty);
    }

    [Fact]
    public void Create_WhenValueIsTooLong_ShouldReturnTooLongError()
    {
        // Arrange
        var value = new string('a', 31);

        // Act
        var result = CategoryName.Create(value);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CategoryName.TooLong);
    }

    [Theory, MemberData(nameof(ValidCategoryNameValues))]
    public void Create_WhenValueIsValid_ShouldReturnCategoryName(string value)
    {
        // Arrange
        // Act
        var result = CategoryName.Create(value);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(value);
    }
}