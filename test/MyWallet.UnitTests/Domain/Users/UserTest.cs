using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Events;

namespace MyWallet.UnitTests.Domain.Users;

[TestSubject(typeof(User))]
public sealed class UserTest
{
    private readonly IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IEmailUniquenessChecker emailUniquenessChecker = Substitute.For<IEmailUniquenessChecker>();

    [Fact]
    public async Task CreateAsync_WhenCalled_ShouldRaiseUserCreatedEvent()
    {
        // Arrange
        // Act
        var result = await Factories.User.CreateDefault();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Events.Should().ContainSingle(e => e is UserCreatedEvent);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailIsNotUnique_ShouldReturnEmailNotUnique()
    {
        // Arrange
        var email = Email.Create("jane@doe.com").Value;

        emailUniquenessChecker.IsUniqueAsync(email, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await Factories.User.CreateDefault(emailUniquenessChecker: emailUniquenessChecker);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.EmailNotUnique);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_ShouldSetPasswordHash()
    {
        // Arrange
        var password = Password.Create("S0m3P4ssw0rd").Value;
        const string passwordHash = "hashed-password";

        passwordHasher.Hash(password)
            .Returns(passwordHash);

        // Act
        var result = await Factories.User.CreateDefault(
            password: password,
            passwordHasher: passwordHasher);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.PasswordHash.Should().Be(passwordHash);
    }

    [Fact]
    public async Task VerifyPassword_WhenPasswordIsCorrect_ShouldReturnTrue()
    {
        // Arrange
        var password = Password.Create("S0m3P4ssw0rd").Value;

        var user = await Factories.User.CreateDefault(
            password: password,
            passwordHasher: passwordHasher);

        passwordHasher.Verify(password, user.Value.PasswordHash)
            .Returns(true);

        // Act
        var result = user.Value.VerifyPassword(password, passwordHasher);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyPassword_WhenPasswordIsIncorrect_ShouldReturnFalse()
    {
        // Arrange
        var password = Password.Create("S0m3P4ssw0rd").Value;

        var user = await Factories.User.CreateDefault(
            password: password,
            passwordHasher: passwordHasher);

        passwordHasher.Verify(password, user.Value.PasswordHash)
            .Returns(false);

        // Act
        var result = user.Value.VerifyPassword(password, passwordHasher);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ChangeEmailAsync_WhenPasswordIsIncorrect_ShouldReturnInvalidPassword()
    {
        // Arrange
        var password = Constants.User.Password;
        var oldEmail = Constants.User.Email;
        var newEmail = Constants.User.Email2;

        passwordHasher.Verify(password, Arg.Any<string>())
            .Returns(false);


        var user = await Factories.User.CreateDefault(
            email: oldEmail,
            password: password);

        // Act
        var result = await user.Value.ChangeEmailAsync(
            password: Constants.User.Password,
            newEmail: newEmail,
            passwordHasher: passwordHasher,
            emailUniquenessChecker: emailUniquenessChecker);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidPassword);

        emailUniquenessChecker
            .ReceivedWithAnyArgs(Quantity.None());
    }

    [Fact]
    public async Task ChangeEmailAsync_WhenNewEmailIsEqualToOldEmail_ShouldReturnCannotChangeToSameEmail()
    {
        // Arrange
        var password = Constants.User.Password;
        var oldEmail = Constants.User.Email;
        var newEmail = Constants.User.Email;

        passwordHasher.Verify(password, Arg.Any<string>())
            .Returns(true);

        var user = await Factories.User.CreateDefault(
            email: oldEmail,
            password: password);

        // Act
        var result = await user.Value.ChangeEmailAsync(
            password: Constants.User.Password,
            newEmail: newEmail,
            passwordHasher: passwordHasher,
            emailUniquenessChecker: emailUniquenessChecker);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.CannotChangeToSameEmail);

        emailUniquenessChecker
            .ReceivedWithAnyArgs(Quantity.None());
    }

    [Fact]
    public async Task ChangeEmailAsync_WhenNewEmailIsNotUnique_ShouldReturnEmailNotUnique()
    {
        // Arrange
        var password = Constants.User.Password;
        var oldEmail = Constants.User.Email;
        var newEmail = Constants.User.Email2;


        emailUniquenessChecker.IsUniqueAsync(newEmail, Arg.Any<CancellationToken>())
            .Returns(false);

        passwordHasher.Verify(password, Arg.Any<string>())
            .Returns(true);

        var user = await Factories.User.CreateDefault(
            password: password,
            email: oldEmail);

        // Act
        var result = await user.Value.ChangeEmailAsync(
            password: Constants.User.Password,
            newEmail: newEmail,
            passwordHasher: passwordHasher,
            emailUniquenessChecker: emailUniquenessChecker);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.EmailNotUnique);
    }

    [Fact]
    public async Task ChangeEmailAsync_WhenCalled_ShouldChangeEmail()
    {
        // Arrange
        var password = Constants.User.Password;
        var oldEmail = Constants.User.Email;
        var newEmail = Constants.User.Email2;

        emailUniquenessChecker.IsUniqueAsync(newEmail, Arg.Any<CancellationToken>())
            .Returns(true);

        passwordHasher.Verify(password, Arg.Any<string>())
            .Returns(true);

        var user = await Factories.User.CreateDefault(
            password: password,
            email: oldEmail);

        // Act
        var result = await user.Value.ChangeEmailAsync(
            password: Constants.User.Password,
            newEmail: newEmail,
            passwordHasher: passwordHasher,
            emailUniquenessChecker: emailUniquenessChecker);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
        user.Value.Email.Should().Be(newEmail);
    }

    [Fact]
    public async Task ChangePassword_WhenNewPasswordIsEqualToOldPassword_ShouldReturnCannotChangeToSamePassword()
    {
        // Arrange
        var password = Password.Create("S0m3P4ssw0rd").Value;
        var user = await Factories.User.CreateDefault(
            password: password,
            passwordHasher: passwordHasher);

        // Act
        var result = user.Value.ChangePassword(password, password, passwordHasher);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.CannotChangeToSamePassword);
    }

    [Fact]
    public async Task ChangePassword_WhenOldPasswordIsIncorrect_ShouldReturnInvalidPassword()
    {
        // Arrange
        var oldPassword = Password.Create("S0m3P4ssw0rd").Value;
        var newPassword = Password.Create("N3wP4ssw0rd").Value;

        var user = await Factories.User.CreateDefault(
            password: oldPassword,
            passwordHasher: passwordHasher);

        passwordHasher.Verify(oldPassword, user.Value.PasswordHash)
            .Returns(false);

        // Act
        var result = user.Value.ChangePassword(oldPassword, newPassword, passwordHasher);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidPassword);
    }

    [Fact]
    public async Task ChangePassword_WhenOldPasswordIsCorrect_ShouldChangePassword()
    {
        // Arrange
        var oldPassword = Password.Create("S0m3P4ssw0rd").Value;
        var newPassword = Password.Create("N3wP4ssw0rd").Value;
        const string newPasswordHash = "new-hashed-password";

        var user = await Factories.User.CreateDefault(
            password: oldPassword,
            passwordHasher: passwordHasher);

        passwordHasher.Verify(oldPassword, user.Value.PasswordHash)
            .Returns(true);

        passwordHasher.Hash(newPassword)
            .Returns(newPasswordHash);

        // Act
        var result = user.Value.ChangePassword(oldPassword, newPassword, passwordHasher);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
        user.Value.PasswordHash.Should().Be(newPasswordHash);
    }
}