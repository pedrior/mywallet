using MyWallet.Domain.Transactions;
using MyWallet.Shared.Persistence;

namespace MyWallet.Features.Transactions.Get;

public sealed class GetTransactionHandler(IDbContext db) 
    : IQueryHandler<GetTransactionQuery, GetTransactionResponse>
{
    public async Task<ErrorOr<GetTransactionResponse>> Handle(GetTransactionQuery query,
        CancellationToken cancellationToken)
    {
        var response = await db.QuerySingleOrDefaultAsync<GetTransactionResponse>(
            sql: """
                     SELECT
                        t.id,
                        t.wallet_id,
                        t.category_id,
                        (SELECT c.name FROM categories c WHERE c.id = t.category_id) as category_name,
                        t.type,
                        t.name,
                        t.amount,
                        t.currency,
                        t.date,
                        t.created_at,
                        t.updated_at
                     FROM transactions t
                     WHERE t.id = @id
                 """,
            param: new { id = query.TransactionId },
            cancellationToken);

        return response is not null ? response : TransactionErrors.NotFound;
    }
}