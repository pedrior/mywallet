using MyWallet.Domain.Users;

namespace MyWallet.Features.Users.Register;

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