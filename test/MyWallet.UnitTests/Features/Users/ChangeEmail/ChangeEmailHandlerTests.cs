using MyWallet.Domain.Users;
using MyWallet.Features.Users.ChangeEmail;

namespace MyWallet.UnitTests.Features.Users.ChangeEmail;

public sealed class ChangeEmailHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IEmailUniquenessChecker emailUniquenessChecker = Substitute.For<IEmailUniquenessChecker>();

    private readonly ChangeEmailHandler sut;

    private static readonly ChangeEmailCommand Command = new()
    {
        NewEmail = Constants.User.Email2.Value,
        Password = Constants.User.Password.Value,
        UserId = Constants.User.Id.Value
    };

    public ChangeEmailHandlerTests()
    {
        sut = new ChangeEmailHandler(userRepository, passwordHasher, emailUniquenessChecker);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnSuccess()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        userRepository.GetAsync(Constants.User.Id, Arg.Any<CancellationToken>())
            .Returns(user.Value);

        passwordHasher.Verify(Constants.User.Password, user.Value.PasswordHash)
            .Returns(true);

        emailUniquenessChecker.IsUniqueAsync(Constants.User.Email2, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await sut.Handle(Command, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldUpdateUserEmail()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        userRepository.GetAsync(Constants.User.Id, Arg.Any<CancellationToken>())
            .Returns(user.Value);

        passwordHasher.Verify(Constants.User.Password, user.Value.PasswordHash)
            .Returns(true);

        emailUniquenessChecker.IsUniqueAsync(Constants.User.Email2, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await sut.Handle(Command, default);

        // Assert
        user.Value.Email.Should().Be(Constants.User.Email2);

        await userRepository
            .Received(1)
            .UpdateAsync(user.Value, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCalledWithInvalidPassword_ShouldReturnError()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        userRepository.GetAsync(Constants.User.Id, Arg.Any<CancellationToken>())
            .Returns(user.Value);

        passwordHasher.Verify(Constants.User.Password, user.Value.PasswordHash)
            .Returns(false);

        // Act
        var result = await sut.Handle(Command, default);

        // Assert
        result.IsError.Should().BeTrue();

        await userRepository
            .DidNotReceive()
            .UpdateAsync(user.Value, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCalledWithNonUniqueEmail_ShouldReturnError()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        userRepository.GetAsync(Constants.User.Id, Arg.Any<CancellationToken>())
            .Returns(user.Value);

        passwordHasher.Verify(Constants.User.Password, user.Value.PasswordHash)
            .Returns(true);

        emailUniquenessChecker.IsUniqueAsync(Constants.User.Email2, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await sut.Handle(Command, default);

        // Assert
        result.IsError.Should().BeTrue();

        await userRepository
            .DidNotReceive()
            .UpdateAsync(user.Value, Arg.Any<CancellationToken>());
    }
}