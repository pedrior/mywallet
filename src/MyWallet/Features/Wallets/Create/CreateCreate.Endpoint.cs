using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Create;

public sealed class CreateWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("wallets", CreateWalletAsync)
            .RequireAuthorization();

    private static Task<IResult> CreateWalletAsync(
        CreateWalletRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(), cancellationToken)
            .ToResponseAsync(
                id => Results.CreatedAtRoute(
                    routeName: "GetWallet",
                    routeValues: new { id }),
                context);
    }
}