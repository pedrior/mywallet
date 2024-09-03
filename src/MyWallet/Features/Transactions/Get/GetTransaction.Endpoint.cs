using MyWallet.Shared.Features;

namespace MyWallet.Features.Transactions.Get;

public sealed class GetTransactionEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("transactions/{id:length(26)}", GetTransactionAsync)
            .RequireAuthorization()
            .WithName("GetTransaction");

    private static Task<IResult> GetTransactionAsync(
        Ulid id,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetTransactionQuery
        {
            TransactionId = id
        };

        return sender.Send(query, cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}