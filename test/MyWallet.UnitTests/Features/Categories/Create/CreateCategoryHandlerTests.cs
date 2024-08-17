using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Create;

namespace MyWallet.UnitTests.Features.Categories.Create;

[TestSubject(typeof(CreateCategoryHandler))]
public sealed class CreateCategoryHandlerTests
{
    private readonly ICategoryRepository categoryRepository = A.Fake<ICategoryRepository>();

    private readonly CreateCategoryHandler sut;

    private static readonly CreateCategoryCommand Command = new()
    {
        Type = "expense",
        Name = "Shopping",
        Color = "#EF5350",
        UserId = Ulid.NewUlid()
    };

    public CreateCategoryHandlerTests()
    {
        sut = new CreateCategoryHandler(categoryRepository);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldReturnCategoryId()
    {
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<CategoryId>()
            .Which.Value.Should().NotBe(Ulid.Empty);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldPersistCategory()
    {
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => categoryRepository.AddAsync(
                A<Category>.That.Matches(v => v.Id == result.Value),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}