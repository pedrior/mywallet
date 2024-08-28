using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;

namespace MyWallet.Shared.Persistence.Repositories;

public sealed class CategoryRepository(IDbContext context, IPublisher publisher)
    : Repository<Category, CategoryId>(context, publisher), ICategoryRepository
{
    public override Task<Category?> GetAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        return Context.QuerySingleOrDefaultAsync<Category>(
            sql: """
                 SELECT c.*
                 FROM categories c
                 WHERE c.id = @id
                 """,
            param: new { id },
            cancellationToken: cancellationToken);
    }

    public override Task<bool> ExistsAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        return Context.ExecuteScalarAsync<bool>(
            sql: """
                  SELECT 1
                  FROM categories c
                  WHERE c.id = @id
                  """,
            param: new { id },
            cancellationToken: cancellationToken);
    }

    public override async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                  INSERT INTO categories (id,
                                          user_id,
                                          type,
                                          name,
                                          color,
                                          created_at)
                  VALUES (@Id,
                          @UserId,
                          @Type::CATEGORY_TYPE,
                          @Name,
                          @Color,
                          NOW() AT TIME ZONE 'UTC')
                  """,
            param: category,
            cancellationToken: cancellationToken);

        await base.AddAsync(category, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Category> categories, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                  INSERT INTO categories (id,
                                          user_id,
                                          type,
                                          name,
                                          color,
                                          created_at)
                  VALUES (@Id,
                          @UserId,
                          @Type::CATEGORY_TYPE,
                          @Name,
                          @Color,
                          NOW() AT TIME ZONE 'UTC')
                  """,
            param: categories,
            cancellationToken: cancellationToken);
        
        foreach (var category in categories)
        {
            await PublishEventsAsync(category, cancellationToken);
        }
    }
    
    public override async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        await Context.ExecuteAsync(
            sql: """
                 UPDATE categories
                 SET name = @Name,
                     color = @Color,
                     updated_at = NOW() AT TIME ZONE 'UTC'
                 WHERE id = @Id
                 """,
            param: category,
            cancellationToken: cancellationToken);

        await base.UpdateAsync(category, cancellationToken);
    }

    public override Task DeleteAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        return Context.ExecuteAsync(
            sql: """
                 DELETE
                 FROM categories c 
                 WHERE c.id = @id
                 """,
            param: new { id },
            cancellationToken: cancellationToken);
    }

    public Task<bool> IsOwnedByUserAsync(
        CategoryId categoryId,
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return Context.ExecuteScalarAsync<bool>(
            sql: """
                 SELECT 1
                 FROM categories c
                 WHERE c.id = @categoryId
                   AND c.user_id = @userId
                 """,
            param: new { categoryId, userId },
            cancellationToken: cancellationToken);
    }
}