using MyWallet.Domain.Transactions;

namespace MyWallet.Shared.Persistence.Repositories;

public sealed class TransactionRepository(IDbContext context, IPublisher publisher)
    : Repository<Transaction, TransactionId>(context, publisher), ITransactionRepository
{
    public override Task<Transaction?> GetAsync(TransactionId id, CancellationToken cancellationToken = default)
    {
        return Context.QuerySingleOrDefaultAsync<Transaction>(
            sql: """
                    SELECT t.*
                    FROM transactions t
                    WHERE t.id = @id
                 """,
            param: new { id },
            cancellationToken);
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
                        date = @Date:DATE,
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