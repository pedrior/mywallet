using MyWallet.Domain.Users;
using MyWallet.Features.Users;

namespace MyWallet.UnitTests.Features.Users;

public sealed class ChangeEmailHandlerTests
{
    private readonly IUserRepository userRepository = A.Fake<IUserRepository>();
    private readonly IPasswordHasher passwordHasher = A.Fake<IPasswordHasher>();
    private readonly IEmailUniquenessChecker emailUniquenessChecker = A.Fake<IEmailUniquenessChecker>();

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

        A.CallTo(() => userRepository.GetAsync(Constants.User.Id, A<CancellationToken>._))
            .Returns(user.Value);

        A.CallTo(() => passwordHasher.Verify(Constants.User.Password, user.Value.PasswordHash))
            .Returns(true);

        A.CallTo(() => emailUniquenessChecker.IsUniqueAsync(Constants.User.Email2, A<CancellationToken>._))
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

        A.CallTo(() => userRepository.GetAsync(Constants.User.Id, A<CancellationToken>._))
            .Returns(user.Value);

        A.CallTo(() => passwordHasher.Verify(Constants.User.Password, user.Value.PasswordHash))
            .Returns(true);

        A.CallTo(() => emailUniquenessChecker.IsUniqueAsync(Constants.User.Email2, A<CancellationToken>._))
            .Returns(true);

        // Act
        await sut.Handle(Command, default);

        // Assert
        user.Value.Email.Should().Be(Constants.User.Email2);

        A.CallTo(() => userRepository.UpdateAsync(user.Value, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenCalledWithInvalidPassword_ShouldReturnError()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        A.CallTo(() => userRepository.GetAsync(Constants.User.Id, A<CancellationToken>._))
            .Returns(user.Value);

        A.CallTo(() => passwordHasher.Verify(Constants.User.Password, user.Value.PasswordHash))
            .Returns(false);

        // Act
        var result = await sut.Handle(Command, default);

        // Assert
        result.IsError.Should().BeTrue();

        A.CallTo(() => userRepository.UpdateAsync(user.Value, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenCalledWithNonUniqueEmail_ShouldReturnError()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        A.CallTo(() => userRepository.GetAsync(Constants.User.Id, A<CancellationToken>._))
            .Returns(user.Value);

        A.CallTo(() => passwordHasher.Verify(Constants.User.Password, user.Value.PasswordHash))
            .Returns(true);

        A.CallTo(() => emailUniquenessChecker.IsUniqueAsync(Constants.User.Email2, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await sut.Handle(Command, default);

        // Assert
        result.IsError.Should().BeTrue();

        A.CallTo(() => userRepository.UpdateAsync(user.Value, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}