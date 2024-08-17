using MyWallet.Domain.Users;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Users.UpdateProfile;

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(UserName.Validate);
    }
}