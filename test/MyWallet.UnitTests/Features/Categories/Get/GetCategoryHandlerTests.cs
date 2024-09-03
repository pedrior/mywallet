using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Get;
using MyWallet.Features.Categories.Shared;
using MyWallet.Shared.Persistence;

namespace MyWallet.UnitTests.Features.Categories.Get;

[TestSubject(typeof(GetCategoryHandler))]
public sealed class GetCategoryHandlerTests
{
    private readonly IDbContext dbContext = Substitute.For<IDbContext>();

    private readonly GetCategoryHandler sut;

    private static readonly GetCategoryQuery Query = new(Ulid.NewUlid());

    private readonly Category category = Factories.Category.CreateDefault(
        id: new CategoryId(Query.CategoryId));

    public GetCategoryHandlerTests()
    {
        sut = new GetCategoryHandler(dbContext);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnCategoryResponse()
    {
        // Arrange
        dbContext.QuerySingleOrDefaultAsync<CategoryResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(new CategoryResponse
            {
                Id = category.Id.Value,
                Type = category.Type.Name,
                Name = category.Name.Value,
                Color = category.Color.Value,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            });
        
        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(new
        {
            Id = category.Id.Value,
            Name = category.Name.Value,
            Color = category.Color.Value,
            category.CreatedAt,
            category.UpdatedAt
        });
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        dbContext.QuerySingleOrDefaultAsync<CategoryResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CategoryErrors.NotFound);
    }
}