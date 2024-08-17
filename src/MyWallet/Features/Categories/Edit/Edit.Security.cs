using MyWallet.Features.Categories.Shared.Security;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Categories.Edit;

public sealed class EditCategoryAuthorizer : IAuthorizer<EditCategoryCommand>
{
    public IEnumerable<IRequirement> GetRequirements(EditCategoryCommand command)
    {
        yield return new CategoryOwnerRequirement(command.UserId, command.CategoryId);
    }
}