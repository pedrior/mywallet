using MyWallet.Domain.Users;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Users;

public sealed record ChangeEmailRequest
{
    public required string NewEmail { get; init; }

    public required string Password { get; init; }
}

public sealed record ChangeEmailCommand : ICommand<Success>, IHaveUser
{
    public required string NewEmail { get; init; }

    public required string Password { get; init; }

    public Ulid UserId { get; set; }
}

public sealed class ChangeEmailEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("users/me/change-email", ChangeEmailAsync)
            .RequireAuthorization();

    private static Task<IResult> ChangeEmailAsync(
        ChangeEmailRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new ChangeEmailCommand
        {
            NewEmail = request.NewEmail,
            Password = request.Password
        };

        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}

public sealed class ChangeEmailValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailValidator()
    {
        RuleFor(c => c.NewEmail)
            .MustSatisfyErrorValidation(Email.Validate);

        RuleFor(c => c.Password)
            .MustSatisfyErrorValidation(Password.Validate);
    }
}

public sealed class ChangeEmailHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IEmailUniquenessChecker emailUniquenessChecker)
    : ICommandHandler<ChangeEmailCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(ChangeEmailCommand command,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(command.UserId);
        var user = await userRepository.GetAsync(userId, cancellationToken);

        var newEmail = Email.Create(command.NewEmail).Value;
        var password = Password.Create(command.Password).Value;

        var result = await user!.ChangeEmailAsync(
            password,
            newEmail,
            passwordHasher,
            emailUniquenessChecker,
            cancellationToken);

        return await result
            .ThenDoAsync(_ => userRepository.UpdateAsync(user, cancellationToken));
    }
}