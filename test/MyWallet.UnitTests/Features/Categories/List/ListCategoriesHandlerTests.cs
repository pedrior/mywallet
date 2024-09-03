using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.List;
using MyWallet.Shared.Persistence;

namespace MyWallet.UnitTests.Features.Categories.List;

[TestSubject(typeof(ListCategoriesHandler))]
public sealed class ListCategoriesHandlerTests
{
    private readonly IDbContext dbContext = Substitute.For<IDbContext>();

    private readonly ListCategoriesHandler sut;

    private static readonly ListCategoriesQuery Query = new();

    public ListCategoriesHandlerTests()
    {
        sut = new ListCategoriesHandler(dbContext);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnListOfListCategoryResponse()
    {
        // Arrange
        Category[] categories =
        [
            Factories.Category.CreateDefault(id: CategoryId.New()),
            Factories.Category.CreateDefault(id: CategoryId.New()),
            Factories.Category.CreateDefault(id: CategoryId.New())
        ];

        dbContext.QueryAsync<ListCategoriesResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(categories.Select(c => new ListCategoriesResponse
            {
                Id = c.Id.Value,
                Type = c.Type.Name,
                Name = c.Name.Value,
                Color = c.Color.Value
            }));

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(categories.Select(c => new
        {
            Id = c.Id.Value,
            Name = c.Name.Value,
            Color = c.Color.Value
        }));
    }

    [Fact]
    public async Task Handle_WhenNoCategoriesExist_ShouldReturnEmptyList()
    {
        // Arrange
        dbContext.QueryAsync<ListCategoriesResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}