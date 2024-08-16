using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Security;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets;

public sealed record DeleteWalletCommand(Ulid WalletId) : ICommand<Deleted>, IHaveUser
{
    public Ulid UserId { get; set; }
}

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

public sealed class DeleteWalletAuthorizer : IAuthorizer<DeleteWalletCommand>
{
    public IEnumerable<IRequirement> GetRequirements(DeleteWalletCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
    }
}

public sealed class DeleteWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<DeleteWalletCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteWalletCommand commnad,
        CancellationToken cancellationToken)
    {
        await walletRepository.DeleteAsync(new WalletId(commnad.WalletId), cancellationToken);
        return Result.Deleted;
    }
}