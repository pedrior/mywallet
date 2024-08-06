using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Shared.Identity;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Categories.Security;

public sealed class CategoryOwnerRequirementHandler(
    IUserContext userContext,
    ICategoryRepository categoryRepository) : IRequirementHandler<CategoryOwnerRequirement>
{
    public async Task<ErrorOr<Success>> HandleAsync(CategoryOwnerRequirement requirement,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(userContext.Id);
        var categoryId = new CategoryId(requirement.CategoryId);

        if (!await categoryRepository.ExistsAsync(categoryId, cancellationToken))
        {
            return requirement.ResourceNotFoundFallbackError;
        }

        return await categoryRepository.IsOwnedByUserAsync(categoryId, userId, cancellationToken)
            ? Result.Success
            : requirement.Forbidden;
    }
}