using MyWallet.Domain.Users;
using MyWallet.Shared.Persistence;

namespace MyWallet.Shared.Services;

public sealed class EmailUniquenessChecker(IDbContext db) : IEmailUniquenessChecker
{
    public async Task<bool> IsUniqueAsync(Email email, CancellationToken cancellationToken)
    {
        const string sql = """
                               SELECT 1
                               FROM users u
                               WHERE u.email = @email
                           """;

        return !await db.ExecuteScalarAsync<bool>(
            sql: sql,
            param: new { email },
            cancellationToken);
    }
}