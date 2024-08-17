using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Unarchive;

public sealed class UnarchiveWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("wallets/{id:length(26)}/unarchive", UnarchiveWalletAsync)
            .RequireAuthorization();

    private static Task<IResult> UnarchiveWalletAsync(
        Ulid id,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new UnarchiveWalletCommand
        {
            WalletId = id
        };

        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}