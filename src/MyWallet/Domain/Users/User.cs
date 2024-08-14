namespace MyWallet.Domain.Users;

public sealed class User : Entity<UserId>, IAggregateRoot, IAuditable
{
    private User()
    {
    }

    public override required UserId Id { get; init; }

    public Email Email { get; private set; } = null!;

    public UserName Name { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? UpdatedAt { get; }
    // ReSharper restore UnassignedGetOnlyAutoProperty

    public static async Task<ErrorOr<User>> CreateAsync(
        UserId id,
        UserName name,
        Email email,
        Password password,
        IEmailUniquenessChecker emailUniquenessChecker,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken = default)
    {
        if (!await emailUniquenessChecker.IsUniqueAsync(email, cancellationToken))
        {
            return UserErrors.EmailNotUnique;
        }

        return new User
        {
            Id = id,
            Name = name,
            Email = email,
            PasswordHash = passwordHasher.Hash(password)
        };
    }

    public void UpdateProfile(UserName name)
    {
        Name = name;
    }

    public bool VerifyPassword(Password password, IPasswordHasher passwordHasher) =>
        passwordHasher.Verify(password, PasswordHash);

    public async Task<ErrorOr<Success>> ChangeEmailAsync(
        Password password,
        Email newEmail,
        IPasswordHasher passwordHasher,
        IEmailUniquenessChecker emailUniquenessChecker,
        CancellationToken cancellationToken = default)
    {
        if (!VerifyPassword(password, passwordHasher))
        {
            return UserErrors.InvalidPassword;
        }
        
        if (Email == newEmail)
        {
            return UserErrors.CannotChangeToSameEmail;
        }

        if (!await emailUniquenessChecker.IsUniqueAsync(newEmail, cancellationToken))
        {
            return UserErrors.EmailNotUnique;
        }

        Email = newEmail;
        
        return Result.Success;
    }

    public ErrorOr<Success> ChangePassword(
        Password oldPassword,
        Password newPassword,
        IPasswordHasher passwordHasher)
    {
        if (oldPassword == newPassword)
        {
            return UserErrors.CannotChangeToSamePassword;
        }

        if (!VerifyPassword(oldPassword, passwordHasher))
        {
            return UserErrors.InvalidPassword;
        }

        PasswordHash = passwordHasher.Hash(newPassword);

        return Result.Success;
    }
}