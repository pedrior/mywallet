using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Categories.Security;

public sealed class CategoryOwnerRequirementHandler(ICategoryRepository categoryRepository) 
    : IRequirementHandler<CategoryOwnerRequirement>
{
    public async Task<ErrorOr<Success>> HandleAsync(CategoryOwnerRequirement requirement,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(requirement.UserId);
        var categoryId = new CategoryId(requirement.CategoryId);

        if (!await categoryRepository.ExistsAsync(categoryId, cancellationToken))
        {
            return requirement.ResourceNotFoundFallbackError;
        }

        return await categoryRepository.IsOwnedByUserAsync(categoryId, userId, cancellationToken)
            ? requirement.Allow()
            : requirement.Forbid();
    }
}