using MyWallet.Features.Categories.Errors;
using MyWallet.Features.Categories.Security;
using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;
using MyWallet.Shared.Security;
using MyWallet.Shared.Errors;

namespace MyWallet.Features.Categories;

public sealed record CategoryResponse
{
    public required Ulid Id { get; init; }

    public required string Type { get; init; }
    
    public required string Name { get; init; }

    public required string Color { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }
}

public sealed record GetCategoryQuery(Ulid CategoryId) : IQuery<CategoryResponse>, IHaveUser
{
    public Ulid UserId { get; set; }
}

public sealed class GetCategoryEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("categories/{id:length(26)}", GetCategoryAsync)
            .RequireAuthorization()
            .WithName("GetCategory");

    private static Task<IResult> GetCategoryAsync(
        Ulid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new GetCategoryQuery(id), cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}

public sealed class GetCategoryAuthorizer : IAuthorizer<GetCategoryQuery>
{
    public IEnumerable<IRequirement> GetRequirements(GetCategoryQuery query)
    {
        yield return new CategoryOwnerRequirement(query.UserId, query.CategoryId);
    }
}

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