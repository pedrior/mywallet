using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;

namespace MyWallet.Shared.Security;

public sealed class CategoryOwnerRequirement(Ulid userId, Ulid categoryId) : ResourceRequirement
{
    public Ulid UserId => userId;
    
    public Ulid CategoryId => categoryId;

    protected override string ResourceName => "Category";
    
    protected override string ForbiddenDescription => "You are not the owner of this category.";
}

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
            return requirement.ResourceNotFound();
        }

        return await categoryRepository.IsOwnedByUserAsync(categoryId, userId, cancellationToken)
            ? requirement.Allow()
            : requirement.Forbid();
    }
}