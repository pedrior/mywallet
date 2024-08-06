using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;

namespace MyWallet.Shared.Persistence.Repositories;

public sealed class UserRepository(IDbContext context, IPublisher publisher)
    : Repository<User, UserId>(context, publisher), IUserRepository
{
    public override async Task<User?> GetAsync(UserId id, CancellationToken cancellationToken = default)
    {
        await using var multi = await Context.QueryMultipleAsync(
            sql: """
                 SELECT u.*
                 FROM users u
                 WHERE u.id = @id;

                 SELECT uc.category_id
                 FROM user_categories uc
                 WHERE uc.user_id = @id;
                 """,
            param: new { id },
            cancellationToken: cancellationToken);

        var user = multi.ReadSingleOrDefault<User>();
        if (user is null)
        {
            return null;
        }

        foreach (var categoryId in multi.Read<CategoryId>().ToList())
        {
            user.AddCategory(categoryId);
        }

        return user;
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
                         @CreatedAt)
                 """,
            param: user,
            cancellationToken: cancellationToken);

        await Context.ExecuteAsync(
            sql: """
                 INSERT INTO user_categories (user_id, category_id)
                 VALUES (@UserId, @CategoryId)
                 """,
            param: user.CategoryIds.Select(categoryId => new
            {
                UserId = user.Id,
                CategoryId = categoryId
            }),
            cancellationToken: cancellationToken);

        await base.AddAsync(user, cancellationToken);
    }

    public override async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                 INSERT INTO user_categories (user_id, category_id)
                 VALUES (@UserId, @CategoryId)
                 ON CONFLICT (user_id, category_id) DO NOTHING
                 """,
            param: user.CategoryIds.Select(categoryId => new
            {
                UserId = user.Id,
                CategoryId = categoryId
            }),
            cancellationToken: cancellationToken);

        await Context.ExecuteAsync(
            sql: """
                 UPDATE users
                 SET name = @Name,
                     password_hash = @PasswordHash,
                     updated_at = @UpdatedAt
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