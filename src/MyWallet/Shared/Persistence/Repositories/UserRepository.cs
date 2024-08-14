using MyWallet.Domain.Users;

namespace MyWallet.Shared.Persistence.Repositories;

public sealed class UserRepository(IDbContext context, IPublisher publisher)
    : Repository<User, UserId>(context, publisher), IUserRepository
{
    public override Task<User?> GetAsync(UserId id, CancellationToken cancellationToken = default)
    {
       return Context.QuerySingleOrDefaultAsync<User>(
            sql: """
                 SELECT u.*
                 FROM users u
                 WHERE u.id = @id;
                 """,
            param: new { id },
            cancellationToken: cancellationToken);
    }

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return Context.QuerySingleOrDefaultAsync<User>(
            sql: """
                 SELECT u.*
                 FROM users u
                 WHERE u.email = @email
                 """,
            param: new { email },
            cancellationToken: cancellationToken);
    }

    public override Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return Context.ExecuteScalarAsync<bool>(
            sql: """
                 SELECT 1
                 FROM users u
                 WHERE u.id = @id
                 """,
            param: new { id },
            cancellationToken: cancellationToken);
    }

    public override async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                 INSERT INTO users (id,
                                    name,
                                    email, 
                                    password_hash,
                                    created_at)
                 VALUES (@Id,
                         @Name,
                         @Email,
                         @PasswordHash,
                         NOW() AT TIME ZONE 'UTC')
                 """,
            param: user,
            cancellationToken: cancellationToken);

        await base.AddAsync(user, cancellationToken);
    }

    public override async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                 UPDATE users
                 SET name = @Name,
                     email = @Email,
                     password_hash = @PasswordHash,
                     updated_at = NOW() AT TIME ZONE 'UTC'
                 WHERE id = @Id
                 """,
            param: user,
            cancellationToken: cancellationToken);

        await base.UpdateAsync(user, cancellationToken);
    }

    public override Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return Context.ExecuteAsync(
            sql: """
                 DELETE
                 FROM users u 
                 WHERE u.id = @id
                 """,
            param: new { id },
            cancellationToken: cancellationToken);
    }
}