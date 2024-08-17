using MyWallet.Features.Categories.Shared;
using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;

namespace MyWallet.Features.Categories.Get;

public sealed class GetCategoryHandler(IDbContext dbContext)
    : IQueryHandler<GetCategoryQuery, CategoryResponse>
{
    public async Task<ErrorOr<CategoryResponse>> Handle(GetCategoryQuery query,
        CancellationToken cancellationToken)
    {
        var categoryResponse = await dbContext.QuerySingleOrDefaultAsync<CategoryResponse>(
            sql: """
                 SELECT c.id, c.type, c.name, c.color, c.created_at, c.updated_at
                 FROM categories c
                 WHERE c.id = @Id
                 """,
            param: new { Id = query.CategoryId },
            cancellationToken: cancellationToken);

        return categoryResponse is not null
            ? categoryResponse
            : CategoryErrors.NotFound;
    }
}