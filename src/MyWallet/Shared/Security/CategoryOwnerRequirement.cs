using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;

namespace MyWallet.Shared.Security;

public sealed class CategoryOwnerRequirement(Ulid userId, Ulid categoryId) : Requirement
{
    public Ulid UserId => userId;

    public Ulid CategoryId => categoryId;
}

public sealed class CategoryOwnerRequirementHandler(ICategoryRepository categoryRepository)
    : IRequirementHandler<CategoryOwnerRequirement>
{
    public async Task<ErrorOr<Success>> HandleAsync(CategoryOwnerRequirement requirement,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(requirement.UserId);
        var categoryId = new CategoryId(requirement.CategoryId);

        return !await categoryRepository.ExistsAsync(categoryId, cancellationToken)
               || await categoryRepository.IsOwnedByUserAsync(categoryId, userId, cancellationToken)
            ? requirement.Allow()
            : requirement.Forbid();
    }
}