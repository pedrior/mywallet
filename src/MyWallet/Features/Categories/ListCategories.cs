using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;
using MyWallet.Shared.Errors;

namespace MyWallet.Features.Categories;

public sealed record ListCategoriesResponse
{
    public required Ulid Id { get; init; }

    public required string Type { get; init; }
    
    public required string Name { get; init; }

    public required string Color { get; init; }
}

public sealed record ListCategoriesQuery : IQuery<List<ListCategoriesResponse>>, IHaveUser
{
    public Ulid UserId { get; set; }
}

public sealed class ListCategoriesEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("categories", ListCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> ListCategoryAsync(
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new ListCategoriesQuery(), cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}

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