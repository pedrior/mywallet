using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Transactions.Delete;

public sealed class DeleteTransactionEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapDelete("transactions/{id:length(26)}", DeleteTransactionAsync)
            .RequireAuthorization();

    private static Task<IResult> DeleteTransactionAsync(
        Ulid id,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(new DeleteTransactionCommand(id), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}