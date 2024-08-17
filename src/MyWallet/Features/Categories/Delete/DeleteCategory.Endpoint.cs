using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Delete;

public sealed class DeleteCategoryEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapDelete("categories/{id:length(26)}", DeleteCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> DeleteCategoryAsync(
        Ulid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new DeleteCategoryCommand { CategoryId = id }, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}