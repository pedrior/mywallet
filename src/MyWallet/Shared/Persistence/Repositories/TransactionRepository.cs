using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;

namespace MyWallet.Shared.Persistence.Repositories;

public sealed class TransactionRepository(IDbContext context, IPublisher publisher)
    : Repository<Transaction, TransactionId>(context, publisher), ITransactionRepository
{
    public override async Task<ErrorOr<Transaction>> GetAsync(TransactionId id,
        CancellationToken cancellationToken = default)
    {
        var transaction = await Context.QuerySingleOrDefaultAsync<Transaction>(
            sql: """
                    SELECT t.*
                    FROM transactions t
                    WHERE t.id = @id
                 """,
            param: new { id },
            cancellationToken);

        return transaction is not null ? transaction : TransactionErrors.NotFound;
    }

    public async Task<ErrorOr<WalletId>> GetWalletIdAsync(TransactionId transactionId,
        CancellationToken cancellationToken)
    {
        var walletId = await Context.QuerySingleOrDefaultAsync<WalletId>(
            sql: """
                    SELECT t.wallet_id
                    FROM transactions t
                    WHERE t.id = @transactionId
                 """,
            param: new { transactionId },
            cancellationToken);

        return walletId is not null ? walletId : TransactionErrors.WalletNotFound;
    }

    public override Task<bool> ExistsAsync(TransactionId id, CancellationToken cancellationToken = default)
    {
        return Context.ExecuteScalarAsync<bool>(
            sql: """
                    SELECT 1
                    FROM transactions t
                    WHERE t.id = @id
                 """,
            param: new { id },
            cancellationToken);
    }

    public override async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                    INSERT INTO transactions (id,
                                              wallet_id,
                                              category_id,
                                              type,
                                              name,
                                              amount,
                                              currency,
                                              date,
                                              created_at)
                    VALUES (@Id,
                            @WalletId,
                            @CategoryId,
                            @Type::TRANSACTION_TYPE,
                            @Name,
                            @Amount,
                            @Currency,
                            @Date::DATE,
                            NOW() AT TIME ZONE 'UTC')
                 """,
            param: transaction,
            cancellationToken);

        await base.AddAsync(transaction, cancellationToken);
    }

    public override async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                    UPDATE transactions t
                    SET wallet_id = @WalletId,
                        category_id = @CategoryId,
                        type = @Type::TRANSACTION_TYPE,
                        name = @Name,
                        amount = @Amount,
                        currency = @Currency,
                        date = @Date::DATE,
                        updated_at = NOW() AT TIME ZONE 'UTC'
                    WHERE t.id = @Id
                 """,
            param: transaction,
            cancellationToken);

        await base.UpdateAsync(transaction, cancellationToken);
    }

    public override Task DeleteAsync(TransactionId id, CancellationToken cancellationToken = default)
    {
        return Context.ExecuteAsync(
            sql: """
                    DELETE FROM transactions t
                    WHERE t.id = @id
                 """,
            param: new { id },
            cancellationToken);
    }
}