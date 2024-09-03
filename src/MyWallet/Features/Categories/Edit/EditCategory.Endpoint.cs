using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Edit;

public sealed class EditCategoryEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPatch("categories/{id:length(26)}", EditCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> EditCategoryAsync(
        Ulid id,
        EditCategoryRequest request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(id), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}