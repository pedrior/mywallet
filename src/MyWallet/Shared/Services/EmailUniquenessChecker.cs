using MyWallet.Domain.Users.Services;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Shared.Persistence;

namespace MyWallet.Shared.Services;

public sealed class EmailUniquenessChecker(IDbContext dbContext) : IEmailUniquenessChecker
{
    public async Task<bool> IsUniqueAsync(Email email, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT 1
                           FROM users u
                           WHERE u.email = @email
                           """;

        return !await dbContext.ExecuteScalarAsync<bool>(
            sql,
            new { email },
            cancellationToken: cancellationToken);
    }
}