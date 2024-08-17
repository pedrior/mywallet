using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.List;

public sealed class ListCategoriesEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("categories", ListCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> ListCategoryAsync(
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new ListCategoriesQuery(), cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}