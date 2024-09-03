using MyWallet.Domain.Categories;
using MyWallet.Domain.Categories.Events;
using MyWallet.Features.Categories.Delete;

namespace MyWallet.UnitTests.Features.Categories.Delete;

[TestSubject(typeof(DeleteCategoryHandler))]
public sealed class DeleteCategoryHandlerTests
{
    private readonly ICategoryRepository categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly DeleteCategoryHandler sut;

    private static readonly DeleteCategoryCommand Command = new()
    {
        CategoryId = Constants.Category.Id.Value,
        UserId = Ulid.NewUlid()
    };

    public DeleteCategoryHandlerTests()
    {
        sut = new DeleteCategoryHandler(categoryRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnDeleted()
    {
        // Arrange
        var category = Factories.Category.CreateDefault();

        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(c => c.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Deleted>();
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldDeleteCategory()
    {
        // Arrange
        var category = Factories.Category.CreateDefault();

        categoryRepository.GetAsync(
                Arg.Is<CategoryId>(c => c.Value == Command.CategoryId),
                Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        category.Events.Should().ContainSingle(e => e is CategoryDeletedEvent);

        await categoryRepository
            .Received(1)
            .UpdateAsync(category, Arg.Any<CancellationToken>());
    }
}