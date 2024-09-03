using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.Shared.Persistence.Repositories;

public sealed class WalletRepository(IDbContext context, IPublisher publisher)
    : Repository<Wallet, WalletId>(context, publisher), IWalletRepository
{
    public override async Task<ErrorOr<Wallet>> GetAsync(WalletId id, CancellationToken cancellationToken = default)
    {
        var wallet = await Context.QuerySingleOrDefaultAsync<Wallet>(
            sql: """
                    SELECT w.*
                    FROM wallets w
                    WHERE w.id = @id
                 """,
            param: new { id },
            cancellationToken);

        return wallet is not null ? wallet : WalletErrors.NotFound;
    }

    public override Task<bool> ExistsAsync(WalletId id, CancellationToken cancellationToken = default)
    {
        return Context.ExecuteScalarAsync<bool>(
            sql: """
                    SELECT 1
                    FROM wallets w
                    WHERE w.id = @id
                 """,
            param: new { id },
            cancellationToken);
    }

    public override async Task AddAsync(Wallet wallet, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                    INSERT INTO wallets (id,
                                         user_id,
                                         name,
                                         color,
                                         currency,
                                         is_archived,
                                         archived_at,
                                         created_at)
                    VALUES (@Id,
                            @UserId,
                            @Name,
                            @Color,
                            @Currency,
                            @IsArchived,
                            @ArchivedAt,
                            NOW() AT TIME ZONE 'UTC')
                 """,
            param: wallet,
            cancellationToken);

        await base.AddAsync(wallet, cancellationToken);
    }

    public override async Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                    UPDATE wallets w
                    SET name = @Name,
                        color = @Color,
                        currency = @Currency,
                        is_archived = @IsArchived,
                        archived_at = @ArchivedAt,
                        updated_at = NOW() AT TIME ZONE 'UTC'
                    WHERE w.id = @Id
                 """,
            param: wallet,
            cancellationToken);

        await base.UpdateAsync(wallet, cancellationToken);
    }

    public override Task DeleteAsync(WalletId id, CancellationToken cancellationToken = default)
    {
        return Context.ExecuteAsync(
            sql: """
                    DELETE
                    FROM wallets w
                    WHERE w.id = @id
                 """,
            param: new { id },
            cancellationToken);
    }

    public Task<bool> IsOwnedByUserAsync(
        WalletId walletId,
        UserId userId,
        CancellationToken cancellationToken)
    {
        return Context.ExecuteScalarAsync<bool>(
            sql: """
                    SELECT 1
                    FROM wallets w
                    WHERE w.id = @walletId
                      AND w.user_id = @userId
                 """,
            param: new { walletId, userId },
            cancellationToken);
    }
}