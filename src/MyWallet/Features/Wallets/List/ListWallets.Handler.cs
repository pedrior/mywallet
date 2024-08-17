using MyWallet.Shared.Contracts;
using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;

namespace MyWallet.Features.Wallets.List;

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