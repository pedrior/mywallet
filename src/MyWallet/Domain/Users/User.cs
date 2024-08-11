using MyWallet.Domain.Users.Errors;
using MyWallet.Domain.Users.Services;
using MyWallet.Domain.Users.ValueObjects;

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

    public DateTimeOffset CreatedAt { get; private init; }

    public DateTimeOffset? UpdatedAt { get; private set; }

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
            PasswordHash = passwordHasher.Hash(password),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void UpdateProfile(UserName name)
    {
        Name = name;

        SetUpdatedAt();
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

        SetUpdatedAt();

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

        SetUpdatedAt();

        return Result.Success;
    }

    private void SetUpdatedAt() => UpdatedAt = DateTimeOffset.UtcNow;
}