using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Security;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Security;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Wallets;

public sealed record RenameWalletRequest
{
    public required string Name { get; init; }
}

public sealed record RenameWalletCommand : ICommand<Success>, IHaveUser
{
    public required Ulid WalletId { get; init; }

    public required string Name { get; init; }

    public Ulid UserId { get; set; }
}

public sealed class RenameWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("wallets/{id:length(26)}/rename", RenameWalletAsync)
            .RequireAuthorization();

    private static Task<IResult> RenameWalletAsync(
        Ulid id,
        RenameWalletRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new RenameWalletCommand
        {
            WalletId = id,
            Name = request.Name
        };

        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}

public sealed class RenameWalletAuthorizer : IAuthorizer<RenameWalletCommand>
{
    public IEnumerable<IRequirement> GetRequirements(RenameWalletCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
    }
}

public sealed class RenameWalletValidator : AbstractValidator<RenameWalletCommand>
{
    public RenameWalletValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(WalletName.Validate);
    }
}

public sealed class RenameWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<RenameWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(RenameWalletCommand command,
        CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetAsync(new(command.WalletId), cancellationToken);
        var name = WalletName.Create(command.Name).Value;

        return await wallet!.Rename(name)
            .ThenDoAsync(_ => walletRepository.UpdateAsync(wallet, cancellationToken));
    }
}