using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;

namespace MyWallet.Features.Categories.List;

public sealed class ListCategoriesHandler(IDbContext dbContext)
    : IQueryHandler<ListCategoriesQuery, List<ListCategoriesResponse>>
{
    public async Task<ErrorOr<List<ListCategoriesResponse>>> Handle(ListCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        var listCategoryResponses = await dbContext.QueryAsync<ListCategoriesResponse>(
            sql: """
                 SELECT c.id, c.type, c.name, c.color
                 FROM categories c
                 WHERE c.user_id = @UserId
                 ORDER BY c.created_at DESC, c.name
                 """,
            param: query,
            cancellationToken: cancellationToken);

        return listCategoryResponses.ToList();
    }
}