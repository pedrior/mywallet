using MyWallet.Features.Categories.Shared.Security;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Categories.Delete;

public sealed class DeleteCategoryAuthorizer : IAuthorizer<DeleteCategoryCommand>
{
    public IEnumerable<IRequirement> GetRequirements(DeleteCategoryCommand command)
    {
        yield return new CategoryOwnerRequirement(command.UserId, command.CategoryId);
    }
}