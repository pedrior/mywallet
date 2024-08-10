using MyWallet.Domain.Categories;
using MyWallet.Domain.Categories.Events;
using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Features.Categories;

namespace MyWallet.UnitTests.Features.Categories;

[TestSubject(typeof(DeleteCategoryHandler))]
public sealed class DeleteCategoryHandlerTests
{
    private readonly ICategoryRepository categoryRepository = A.Fake<ICategoryRepository>();

    private readonly DeleteCategoryHandler sut;

    private static readonly DeleteCategoryCommand Command = new()
    {
        CategoryId = Constants.Category.Id.Value,
        UserId = Ulid.NewUlid()
    };

    private static readonly Category Category = Factories.Category.CreateDefault();

    public DeleteCategoryHandlerTests()
    {
        A.CallTo(() => categoryRepository.GetAsync(
                A<CategoryId>.That.Matches(c => c.Value == Command.CategoryId),
                A<CancellationToken>._))
            .Returns(Category);

        sut = new DeleteCategoryHandler(categoryRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnDeleted()
    {
        // Arrange
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
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        Category.Events.Should().ContainSingle(e => e is CategoryDeletedEvent);

        A.CallTo(() => categoryRepository.UpdateAsync(Category, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}