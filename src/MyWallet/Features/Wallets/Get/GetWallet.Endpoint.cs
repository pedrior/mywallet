namespace MyWallet.Features.Wallets.Get;

public sealed class GetWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("wallets/{id:length(26)}", GetWalletAsync)
            .RequireAuthorization()
            .WithName("GetWallet");

    private static Task<IResult> GetWalletAsync(
        Ulid id,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(new GetWalletQuery(id), cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}