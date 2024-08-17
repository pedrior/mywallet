using MyWallet.Domain.Users;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.UpdateProfile;

public sealed class UpdateProfileHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateProfileCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var name = UserName.Create(command.Name);
        
        var user = await userRepository.GetAsync(new(command.UserId), cancellationToken);
        user!.UpdateProfile(name.Value);

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success;
    }
}