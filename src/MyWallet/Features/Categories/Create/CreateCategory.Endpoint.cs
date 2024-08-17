using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Create;

public sealed class CreateCategoryEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("categories", CreateCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> CreateCategoryAsync(
        CreateCategoryRequest request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(), cancellationToken)
            .ToResponseAsync(
                id => Results.CreatedAtRoute("GetCategory", new { id }),
                context);
    }
}