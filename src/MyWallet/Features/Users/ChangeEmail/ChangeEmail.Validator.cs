using MyWallet.Domain.Users;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Users.ChangeEmail;

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