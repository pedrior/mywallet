namespace MyWallet.Features.Transactions.List;

public sealed class ListTransactionEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("transactions", ListTransactionsAsync)
            .RequireAuthorization();

    private static Task<IResult> ListTransactionsAsync(
        [AsParameters] ListTransactionsRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToQuery(), cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}