using MyWallet.Domain;
using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Security;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Security;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Wallets;

public sealed record EditWalletRequest
{
    public required string Name { get; init; }
    
    public required string Color { get; init; }
    
    public required string Currency { get; init; }
}

public sealed record EditWalletCommand : ICommand<Success>, IHaveUser
{
    public required Ulid WalletId { get; init; }

    public required string Name { get; init; }
    
    public required string Color { get; init; }
    
    public required string Currency { get; init; }

    public Ulid UserId { get; set; }
}

public sealed class EditWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("wallets/{id:length(26)}/edit", EditWalletAsync)
            .RequireAuthorization();

    private static Task<IResult> EditWalletAsync(
        Ulid id,
        EditWalletRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new EditWalletCommand
        {
            WalletId = id,
            Name = request.Name,
            Color = request.Color,
            Currency = request.Currency,
        };

        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}

public sealed class EditWalletAuthorizer : IAuthorizer<EditWalletCommand>
{
    public IEnumerable<IRequirement> GetRequirements(EditWalletCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
    }
}

public sealed class EditWalletValidator : AbstractValidator<EditWalletCommand>
{
    public EditWalletValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(WalletName.Validate);

        RuleFor(c => c.Color)
            .MustSatisfyErrorValidation(Color.Validate);

        RuleFor(c => c.Currency)
            .Currency();
    }
}

public sealed class EditWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<EditWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(EditWalletCommand command,
        CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetAsync(new(command.WalletId), cancellationToken);
        
        var name = WalletName.Create(command.Name).Value;
        var color = Color.Create(command.Color).Value;
        var currency = Currency.FromName(command.Currency, ignoreCase: true);
        
        return await wallet!.Edit(name, color, currency)
            .ThenDoAsync(_ => walletRepository.UpdateAsync(wallet, cancellationToken));
    }
}