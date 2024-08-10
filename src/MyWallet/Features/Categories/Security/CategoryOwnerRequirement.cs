using MyWallet.Features.Categories.Errors;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Categories.Security;

public sealed class CategoryOwnerRequirement(Ulid userId, Ulid categoryId) : ResourceRequirement
{
    public Ulid UserId => userId;
    
    public Ulid CategoryId => categoryId;
    
    public override Error ResourceNotFoundFallbackError => CategoryErrors.NotFound;
    
    protected override string ForbiddenDescription => "You are not the owner of this category.";
}