using MyWallet.Domain.Users;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.UpdateProfile;

public sealed class UpdateProfileHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateProfileCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var name = UserName.Create(command.Name).Value;
        
        var user = await userRepository.GetAsync(new UserId(command.UserId), cancellationToken);
        return await user
            .ThenDo(u => u.UpdateProfile(name))
            .ThenDoAsync(u => userRepository.UpdateAsync(u, cancellationToken))
            .Then(_ => Result.Success);
    }
}