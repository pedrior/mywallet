using MyWallet.Domain;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Wallets;

public sealed record CreateWalletRequest
{
    public required string Name { get; init; }

    public required string Color { get; init; }

    public required string Currency { get; init; }
}

public sealed record CreateWalletCommand : ICommand<WalletId>, IHaveUser
{
    public required string Name { get; init; }

    public required string Color { get; init; }

    public required string Currency { get; init; }

    public Ulid UserId { get; set; }
}

public sealed class CreateWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("wallets", CreateWalletAsync)
            .RequireAuthorization();

    private static Task<IResult> CreateWalletAsync(
        CreateWalletRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new CreateWalletCommand
        {
            Name = request.Name,
            Color = request.Color,
            Currency = request.Currency
        };

        return sender.Send(command, cancellationToken)
            .ToResponseAsync(id => Results.CreatedAtRoute(
                routeName: "GetWallet",
                routeValues: new { id }));
    }
}

public sealed class CreateWalletValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(WalletName.Validate);

        RuleFor(c => c.Color)
            .MustSatisfyErrorValidation(Color.Validate);

        RuleFor(c => c.Currency)
            .Currency();
    }
}

public sealed class CreateWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<CreateWalletCommand, WalletId>
{
    public async Task<ErrorOr<WalletId>> Handle(CreateWalletCommand command,
        CancellationToken cancellationToken)
    {
        var name = WalletName.Create(command.Name).Value;
        var color = Color.Create(command.Color).Value;
        var currency = Currency.FromName(command.Currency, ignoreCase: true);

        var wallet = Wallet.Create(
            WalletId.New(),
            new UserId(command.UserId),
            name,
            color,
            currency);

        await walletRepository.AddAsync(wallet, cancellationToken);

        return wallet.Id;
    }
}