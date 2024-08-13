using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Security;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets;

public sealed record ArchiveWalletCommand : ICommand<Success>, IHaveUser
{
    public required Ulid WalletId { get; init; }

    public Ulid UserId { get; set; }
}

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

public sealed class ArchiveWalletAuthorizer : IAuthorizer<ArchiveWalletCommand>
{
    public IEnumerable<IRequirement> GetRequirements(ArchiveWalletCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
    }
}

public sealed class ArchiveWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<ArchiveWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(ArchiveWalletCommand command,
        CancellationToken cancellationToken)
    {
        var walletId = new WalletId(command.WalletId);
        var wallet = await walletRepository.GetAsync(walletId, cancellationToken);
        
        return await wallet!.Archive()
            .ThenDoAsync(_ => walletRepository.UpdateAsync(wallet, cancellationToken));
    }
}