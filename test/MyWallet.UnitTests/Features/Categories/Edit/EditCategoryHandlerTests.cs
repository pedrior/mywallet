using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Edit;
using MyWallet.Features.Categories.Shared.Errors;

namespace MyWallet.UnitTests.Features.Categories.Edit;

[TestSubject(typeof(EditCategoryHandler))]
public sealed class EditCategoryHandlerTests
{
    private readonly ICategoryRepository categoryRepository = A.Fake<ICategoryRepository>();

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
        A.CallTo(() => categoryRepository.GetAsync(
                A<CategoryId>.That.Matches(v => v.Value == Command.CategoryId),
                A<CancellationToken>._))
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
        A.CallTo(() => categoryRepository.GetAsync(
                A<CategoryId>.That.Matches(v => v.Value == Command.CategoryId),
                A<CancellationToken>._))
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
        A.CallTo(() => categoryRepository.GetAsync(
                A<CategoryId>.That.Matches(v => v.Value == Command.CategoryId),
                A<CancellationToken>._))
            .Returns(category);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => categoryRepository.UpdateAsync(
                category,
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        A.CallTo(() => categoryRepository.GetAsync(
                A<CategoryId>.That.Matches(v => v.Value == Command.CategoryId),
                A<CancellationToken>._))
            .Returns(null as Category);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CategoryErrors.NotFound);
    }
}