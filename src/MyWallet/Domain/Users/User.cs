using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.Errors;
using MyWallet.Domain.Users.Events;
using MyWallet.Domain.Users.Services;
using MyWallet.Domain.Users.ValueObjects;

namespace MyWallet.Domain.Users;

public sealed class User : Entity<UserId>, IAggregateRoot, IAuditable
{
    private readonly List<CategoryId> categoryIds = [];

    private User()
    {
    }

    public override required UserId Id { get; init; }

    public required Email Email { get; init; }

    public UserName Name { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public IReadOnlyCollection<CategoryId> CategoryIds => categoryIds.AsReadOnly();

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

    public ErrorOr<Success> AddCategory(CategoryId categoryId)
    {
        if (categoryIds.Contains(categoryId))
        {
            return UserErrors.CategoryAlreadyExists;
        }

        categoryIds.Add(categoryId);

        return Result.Success;
    }

    public ErrorOr<Deleted> DeleteCategory(CategoryId categoryId)
    {
        if (!categoryIds.Remove(categoryId))
        {
            return UserErrors.CategoryNotFound;
        }

        AddEvent(new CategoryDeletedEvent(categoryId));

        return Result.Deleted;
    }
}