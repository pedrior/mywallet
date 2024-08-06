namespace MyWallet.Shared.Persistence;

public interface IDbContext : IDisposable
{
    void BeginTransaction();

    void CommitTransaction();

    void RollbackTransaction();
    
    Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default) where T : class;

    Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default) where T : class;

    Task<SqlMapper.GridReader> QueryMultipleAsync(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default);

    Task<T?> ExecuteScalarAsync<T>(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default);
}