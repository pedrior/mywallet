using MyWallet.Shared.Contracts;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Wallets;

public sealed record ListWalletsRequest
{
    public int? Page { get; init; }

    public int? Limit { get; init; }
}

public sealed record WalletSummaryResponse
{
    public required Ulid Id { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed record ListWalletsQuery : IQuery<PageResponse<WalletSummaryResponse>>, IHaveUser
{
    public required int Page { get; init; }

    public required int Limit { get; init; }

    public Ulid UserId { get; set; }
}

public sealed class ListWalletsEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("wallets", ListWalletsAsync)
            .RequireAuthorization();

    private static Task<IResult> ListWalletsAsync(
        [AsParameters] ListWalletsRequest request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new ListWalletsQuery
        {
            Page = request.Page ?? 1,
            Limit = request.Limit ?? 10
        };

        return sender.Send(query, cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}

public sealed class ListWalletsValidator : AbstractValidator<ListWalletsQuery>
{
    public ListWalletsValidator()
    {
        RuleFor(q => q.Page)
            .PageNumber();

        RuleFor(q => q.Limit)
            .PageLimit();
    }
}

public sealed class ListWalletsHandler(IDbContext db)
    : IQueryHandler<ListWalletsQuery, PageResponse<WalletSummaryResponse>>
{
    public async Task<ErrorOr<PageResponse<WalletSummaryResponse>>> Handle(
        ListWalletsQuery query,
        CancellationToken cancellationToken)
    {
        var total = await CountTotalUserWalletsAsync(query, cancellationToken);
        if (total is 0)
        {
            return PageResponse<WalletSummaryResponse>.Empty(query.Page, query.Limit);
        }

        var items = await ListPaginatedUserWalletsAsync(
            query.UserId,
            query.Limit,
            query.Page,
            cancellationToken);

        return new PageResponse<WalletSummaryResponse>(
            Items: items,
            Page: query.Page,
            Limit: query.Limit,
            Total: total);
    }

    private Task<IEnumerable<WalletSummaryResponse>> ListPaginatedUserWalletsAsync(
        Ulid userId,
        int limit,
        int page,
        CancellationToken cancellationToken)
    {
        return db.QueryAsync<WalletSummaryResponse>(
            sql: """
                    SELECT w.id,
                           w.name,
                           w.color,
                           w.created_at
                    FROM wallets w
                    WHERE w.user_id = @UserId AND w.is_archived = FALSE
                    ORDER BY w.created_at DESC
                    LIMIT @limit OFFSET @offset
                 """,
            param: new
            {
                userId,
                limit,
                offset = (page - 1) * limit
            },
            cancellationToken);
    }

    private Task<int> CountTotalUserWalletsAsync(ListWalletsQuery query,
        CancellationToken cancellationToken)
    {
        return db.ExecuteScalarAsync<int>(
            sql: """
                    SELECT COUNT(*)
                    FROM wallets w
                    WHERE w.user_id = @UserId AND w.is_archived = FALSE
                 """,
            param: new { query.UserId },
            cancellationToken);
    }
}