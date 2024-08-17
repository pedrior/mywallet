using MyWallet.Domain.Users;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Users.Register;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(UserName.Validate);

        RuleFor(c => c.Email)
            .MustSatisfyErrorValidation(Email.Validate);

        RuleFor(c => c.Password)
            .MustSatisfyErrorValidation(Password.Validate);
    }
}