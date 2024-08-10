using MyWallet.Domain.Wallets;
using MyWallet.Domain.Wallets.Repository;
using MyWallet.Domain.Wallets.ValueObjects;

namespace MyWallet.Shared.Persistence.Repositories;

public sealed class WalletRepository(IDbContext context, IPublisher publisher)
    : Repository<Wallet, WalletId>(context, publisher), IWalletRepository
{
    public override Task<Wallet?> GetAsync(WalletId id, CancellationToken cancellationToken = default)
    {
        return Context.QuerySingleOrDefaultAsync<Wallet>(
            sql: """
                    SELECT w.*
                    FROM wallets w
                    WHERE w.id = @id
                 """,
            param: new { id },
            cancellationToken);
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
                                         is_default,
                                         created_at)
                    VALUES (@Id,
                            @UserId,
                            @Name,
                            @Color,
                            @IsDefault,
                            @CreatedAt)
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
                        is_default = @IsDefault,
                        is_archived = @IsArchived,
                        archived_at = @ArchivedAt,
                        updated_at = @UpdatedAt
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
}