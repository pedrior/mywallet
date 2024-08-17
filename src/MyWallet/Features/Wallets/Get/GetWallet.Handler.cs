using MyWallet.Features.Wallets.Shared;
using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;

namespace MyWallet.Features.Wallets.Get;

public sealed class GetWalletHandler(IDbContext db) : IQueryHandler<GetWalletQuery, WalletResponse>
{
    public async Task<ErrorOr<WalletResponse>> Handle(GetWalletQuery query,
        CancellationToken cancellationToken)
    {
        var response = await db.QuerySingleOrDefaultAsync<WalletResponse>(
            sql: """
                    SELECT w.id,
                           w.name,
                           w.color,
                           w.created_at,
                           w.updated_at
                    FROM wallets w
                    WHERE w.id = @Id
                 """,
            param: new { Id = query.WalletId },
            cancellationToken);

        return response is not null
            ? response
            : WalletErrors.NotFound;
    }
}