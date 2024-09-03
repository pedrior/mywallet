using MyWallet.Domain.Users;

namespace MyWallet.Features.Users.ChangeEmail;

public sealed class ChangeEmailHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IEmailUniquenessChecker emailUniquenessChecker)
    : ICommandHandler<ChangeEmailCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(ChangeEmailCommand command,
        CancellationToken cancellationToken)
    {
        var newEmail = Email.Create(command.NewEmail).Value;
        var password = Password.Create(command.Password).Value;
        
        var user = await userRepository.GetAsync(new UserId(command.UserId), cancellationToken);
        if (user.IsError)
        {
            return user.Errors;
        }

        var result = await user.Value.ChangeEmailAsync(
            password,
            newEmail,
            passwordHasher,
            emailUniquenessChecker,
            cancellationToken);

        return await result
            .ThenDoAsync(_ => userRepository.UpdateAsync(user.Value, cancellationToken));
    }
}