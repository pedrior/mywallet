using MyWallet.Domain.Users;
using MyWallet.Shared.Features;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Validations;
using Password = MyWallet.Domain.Users.Password;

namespace MyWallet.Features.Users;

public sealed record ChangePasswordCommand : ICommand<Success>, IHaveUser
{
    public required string OldPassword { get; init; }

    public required string NewPassword { get; init; }
    
    public Ulid UserId { get; set; }
}

public sealed class ChangePasswordEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("users/me/change-password", ChangePasswordAsync)
            .RequireAuthorization();

    private static Task<IResult> ChangePasswordAsync(
        ChangePasswordCommand command,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(c => c.OldPassword)
            .MustSatisfyErrorValidation(Password.Validate);

        RuleFor(c => c.NewPassword)
            .MustSatisfyErrorValidation(Password.Validate);
    }
}

public sealed class ChangePasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher) 
    : ICommandHandler<ChangePasswordCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(new(command.UserId), cancellationToken);
        
        var newPassword = Password.Create(command.NewPassword);
        var oldPassword = Password.Create(command.OldPassword);

        return await user!.ChangePassword(oldPassword.Value, newPassword.Value, passwordHasher)
            .ThenDoAsync(_ => userRepository.UpdateAsync(user, cancellationToken));
    }
}