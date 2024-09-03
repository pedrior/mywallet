using MyWallet.Domain.Users;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.ChangePassword;

public sealed class ChangePasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : ICommandHandler<ChangePasswordCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var newPassword = Password.Create(command.NewPassword).Value;
        var oldPassword = Password.Create(command.OldPassword).Value;

        var user = await userRepository.GetAsync(new UserId(command.UserId), cancellationToken);

        return await user
            .ThenDoOrFail(u => u.ChangePassword(oldPassword, newPassword, passwordHasher))
            .ThenDoAsync(u => userRepository.UpdateAsync(u, cancellationToken))
            .Then(_ => Result.Success);
    }
}