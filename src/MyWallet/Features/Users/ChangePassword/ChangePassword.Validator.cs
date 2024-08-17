using MyWallet.Domain.Users;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Users.ChangePassword;

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