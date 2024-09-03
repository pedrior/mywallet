using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Delete;

public sealed class DeleteWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapDelete("wallets/{id:length(26)}", DeleteWalletAsync)
            .RequireAuthorization();

    private static Task<IResult> DeleteWalletAsync(
        Ulid id,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(new DeleteWalletCommand(id), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}