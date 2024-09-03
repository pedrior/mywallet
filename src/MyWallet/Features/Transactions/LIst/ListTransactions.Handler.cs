using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Persistence;

namespace MyWallet.Features.Transactions.List;

public sealed class ListTransactionsHandler(IWalletRepository walletRepository, IDbContext db)
    : IQueryHandler<ListTransactionsQuery, ListTransactionsResponse>
{
    public async Task<ErrorOr<ListTransactionsResponse>> Handle(ListTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await walletRepository.ExistsAsync(new WalletId(query.WalletId), cancellationToken))
        {
            return TransactionErrors.WalletNotFound;
        }
        
        var total = await CountTotalTransactionsAsync(query, cancellationToken);
        if (total is 0)
        {
            return new ListTransactionsResponse(
                Items: [],
                Page: query.Page,
                Limit: query.Limit,
                Total: total)
            {
                WalletId = query.WalletId,
                From = query.From,
                To = query.To
            };
        }

        var transactions = await ListTransactionsAsync(query, cancellationToken);

        return new ListTransactionsResponse(
            Items: transactions,
            Page: query.Page,
            Limit: query.Limit,
            Total: total)
        {
            WalletId = query.WalletId,
            From = query.From,
            To = query.To
        };
    }

    private Task<int> CountTotalTransactionsAsync(ListTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        return db.ExecuteScalarAsync<int>(
            sql: """
                    SELECT COUNT(*)
                    FROM transactions t
                    WHERE t.wallet_id = @WalletId AND (t.date BETWEEN @From AND @To)
                 """,
            param: query,
            cancellationToken);
    }

    private async Task<IEnumerable<TransactionResponse>> ListTransactionsAsync(ListTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        return await db.QueryAsync<TransactionResponse>(
            sql: """
                    SELECT
                        t.id,
                        t.type,
                        t.name,
                        (SELECT c.name FROM categories c WHERE c.id = t.category_id) AS category,
                        t.amount,
                        t.currency,
                        t.date
                    FROM transactions t
                    WHERE t.wallet_id = @WalletId AND (t.date BETWEEN @From AND @To)
                    ORDER BY t.date DESC, t.created_at DESC
                    LIMIT @Limit OFFSET @Offset
                 """,
            param: query,
            cancellationToken);
    }
}