using MyWallet.Domain.Users;
using MyWallet.Shared.Features;

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