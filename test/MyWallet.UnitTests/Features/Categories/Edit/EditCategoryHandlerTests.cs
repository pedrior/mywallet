using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Edit;
using MyWallet.Features.Categories.Shared;

namespace MyWallet.UnitTests.Features.Categories.Edit;

[TestSubject(typeof(EditCategoryHandler))]
public sealed class EditCategoryHandlerTests
{
    private readonly ICategoryRepository categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly EditCategoryHandler sut;

    private static readonly EditCategoryCommand Command = new()
    {
        CategoryId = Ulid.NewUlid(),
        Name = "Bills",
        Color = "#BCE0FD"
    };

    private readonly Category category = Factories.Category.CreateDefault(
        id: new CategoryId(Command.CategoryId));

    public EditCategoryHandlerTests()
    {
        sut = new EditCategoryHandler(categoryRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnSuccess()
    {
        // Arrange
        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(v => v.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldEditCategory()
    {
        // Arrange
        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(v => v.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        category.Name.Value.Should().Be(Command.Name);
        category.Color.Value.Should().Be(Command.Color);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldUpdateCategory()
    {
        // Arrange
        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(v => v.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await categoryRepository
            .Received(1)
            .UpdateAsync(category, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(v => v.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .Returns(CategoryErrors.NotFound);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CategoryErrors.NotFound);
    }
}