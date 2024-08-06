using MyWallet.Domain.Categories;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Features.Categories;
using MyWallet.Shared.Persistence;

namespace MyWallet.UnitTests.Features.Categories;

[TestSubject(typeof(ListCategoriesHandler))]
public sealed class ListCategoriesHandlerTests
{
    private readonly IDbContext dbContext = A.Fake<IDbContext>();

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

        A.CallTo(() => dbContext.QueryAsync<ListCategoriesResponse>(
                A<string>._,
                A<object>._,
                A<CancellationToken>._))
            .Returns(categories.Select(c => new ListCategoriesResponse
            {
                Id = c.Id.Value,
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
        A.CallTo(() => dbContext.QueryAsync<ListCategoriesResponse>(
                A<string>._,
                A<object>._,
                A<CancellationToken>._))
            .Returns([]);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}