namespace MyWallet.Features.Wallets.Edit;

public sealed class EditWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPatch("wallets/{id:length(26)}", EditWalletAsync)
            .RequireAuthorization();

    private static Task<IResult> EditWalletAsync(
        Ulid id,
        EditWalletRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(id), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}