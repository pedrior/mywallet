using MyWallet.Domain.Users;
using MyWallet.Shared.Features;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Users;

public sealed record RegisterCommand : ICommand<Created>
{
    public required string Name { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }
}

public sealed class RegisterEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("users/register", RegisterAsync);

    private static Task<IResult> RegisterAsync(
        RegisterCommand command,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.Created());
    }
}

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

public sealed class RegisterHandler(
    IUserRepository userRepository,
    IEmailUniquenessChecker emailUniquenessChecker,
    IPasswordHasher passwordHasher) : ICommandHandler<RegisterCommand, Created>
{
    public async Task<ErrorOr<Created>> Handle(RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var name = UserName.Create(command.Name);
        var email = Email.Create(command.Email);
        var password = Password.Create(command.Password);
        
        return await CreateUserAsync(name.Value, email.Value, password.Value, cancellationToken)
            .ThenDoAsync(user => userRepository.AddAsync(user, cancellationToken))
            .Then(_ => Result.Created);
    }

    private Task<ErrorOr<User>> CreateUserAsync(
        UserName name,
        Email email,
        Password password,
        CancellationToken cancellationToken)
    {
        return User.CreateAsync(
            id: UserId.New(),
            name,
            email,
            password,
            emailUniquenessChecker,
            passwordHasher,
            cancellationToken);
    }
}