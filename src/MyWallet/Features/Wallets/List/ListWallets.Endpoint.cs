using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.List;

public sealed class ListWalletsEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("wallets", ListWalletsAsync)
            .RequireAuthorization();

    private static Task<IResult> ListWalletsAsync(
        [AsParameters] ListWalletsRequest request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToQuery(), cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}