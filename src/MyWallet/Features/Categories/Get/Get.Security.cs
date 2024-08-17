using MyWallet.Features.Categories.Shared.Security;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Categories.Get;

public sealed class GetCategoryAuthorizer : IAuthorizer<GetCategoryQuery>
{
    public IEnumerable<IRequirement> GetRequirements(GetCategoryQuery query)
    {
        yield return new CategoryOwnerRequirement(query.UserId, query.CategoryId);
    }
}