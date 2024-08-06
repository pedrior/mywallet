using System.Data;
using Npgsql;

namespace MyWallet.Shared.Persistence;

public sealed class DbContext : IDbContext
{
    private readonly IDbConnection connection;
    private IDbTransaction? transaction;

    public DbContext(string connectionString)
    {
        connection = new NpgsqlConnection(connectionString);
        connection.Open();
    }

    public void BeginTransaction() => transaction = connection.BeginTransaction();

    public void CommitTransaction() => transaction?.Commit();

    public void RollbackTransaction() => transaction?.Rollback();

    public Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default) where T : class
    {
        return connection.QueryAsync<T>(new CommandDefinition(
            sql,
            param,
            cancellationToken: cancellationToken));
    }

    public void Dispose()
    {
        transaction?.Dispose();
        connection.Dispose();
    }

    public Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default) where T : class
    {
        return connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            sql,
            param,
            cancellationToken: cancellationToken));
    }

    public Task<SqlMapper.GridReader> QueryMultipleAsync(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default)
    {
        return connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            param,
            cancellationToken: cancellationToken));
    }

    public Task<int> ExecuteAsync(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default)
    {
        return connection.ExecuteAsync(new CommandDefinition(
            sql,
            param,
            cancellationToken: cancellationToken));
    }

    public Task<T?> ExecuteScalarAsync<T>(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default)
    {
        return connection.ExecuteScalarAsync<T>(new CommandDefinition(
            sql,
            param,
            cancellationToken: cancellationToken));
    }
}