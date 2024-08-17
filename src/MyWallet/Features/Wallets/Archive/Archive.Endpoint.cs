using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Archive;

public sealed class ArchiveWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("wallets/{id:length(26)}/archive", ArchiveWalletAsync)
            .RequireAuthorization();

    private static Task<IResult> ArchiveWalletAsync(
        Ulid id,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new ArchiveWalletCommand
        {
            WalletId = id
        };

        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}