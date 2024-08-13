using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Security;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets;

public sealed record UnarchiveWalletCommand : ICommand<Success>, IHaveUser
{
    public required Ulid WalletId { get; init; }

    public Ulid UserId { get; set; }
}

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

public sealed class UnarchiveWalletAuthorizer : IAuthorizer<UnarchiveWalletCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UnarchiveWalletCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
    }
}

public sealed class UnarchiveWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<UnarchiveWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(UnarchiveWalletCommand command,
        CancellationToken cancellationToken)
    {
        var walletId = new WalletId(command.WalletId);
        var wallet = await walletRepository.GetAsync(walletId, cancellationToken);
        
        return await wallet!.Unarchive()
            .ThenDoAsync(_ => walletRepository.UpdateAsync(wallet, cancellationToken));
    }
}