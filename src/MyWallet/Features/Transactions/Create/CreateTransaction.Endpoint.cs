using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Transactions.Create;

public sealed class CreateTransactionEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("transactions", CreateTransactionAsync)
            .RequireAuthorization();

    private static Task<IResult> CreateTransactionAsync(
        CreateTransactionRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(), cancellationToken)
            .ToResponseAsync(id => Results.CreatedAtRoute(
                    routeName: "GetTransaction",
                    routeValues: new { id }),
                context);
    }
}