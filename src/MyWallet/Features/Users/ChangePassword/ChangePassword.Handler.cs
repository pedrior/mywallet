using MyWallet.Domain.Users;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.ChangePassword;

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