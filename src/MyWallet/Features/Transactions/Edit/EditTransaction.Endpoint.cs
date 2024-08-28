using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Transactions.Edit;

public sealed class EditTransactionEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPatch("transactions/{id:length(26)}", EditCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> EditCategoryAsync(
        Ulid id,
        EditTransactionRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(id), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}