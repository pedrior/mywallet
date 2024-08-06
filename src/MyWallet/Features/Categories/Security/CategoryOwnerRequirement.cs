using MyWallet.Features.Categories.Errors;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Categories.Security;

public sealed class CategoryOwnerRequirement(Ulid categoryId) : ResourceRequirement
{
    public Ulid CategoryId { get; } = categoryId;
    
    public override Error ResourceNotFoundFallbackError => CategoryErrors.NotFound;
}