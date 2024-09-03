using MyWallet.Domain.Users;
using MyWallet.Features.Users.ChangePassword;

namespace MyWallet.UnitTests.Features.Users.ChangePassword;

[TestSubject(typeof(ChangePasswordHandler))]
public sealed class ChangePasswordHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();

    private readonly ChangePasswordHandler sut;

    private static readonly ChangePasswordCommand Command = new()
    {
        OldPassword = "OldJohnP455",
        NewPassword = "NewJohnP455",
        UserId = Ulid.NewUlid()
    };

    public ChangePasswordHandlerTests()
    {
        sut = new ChangePasswordHandler(userRepository, passwordHasher);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var user = await Factories.User.CreateDefault(
            id: new(Command.UserId),
            password: Password.Create(Command.OldPassword).Value,
            passwordHasher: passwordHasher);

        userRepository.GetAsync(user.Value.Id, Arg.Any<CancellationToken>())
            .Returns(user.Value);

        passwordHasher.Verify(
                Arg.Is<Password>(v => v.Value == Command.OldPassword),
                user.Value.PasswordHash)
            .Returns(true);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldChangePassword()
    {
        // Arrange
        var user = await Factories.User.CreateDefault(
            id: new(Command.UserId),
            password: Password.Create(Command.OldPassword).Value,
            passwordHasher: passwordHasher);

        userRepository.GetAsync(user.Value.Id, Arg.Any<CancellationToken>())
            .Returns(user.Value);

        passwordHasher.Hash(Arg.Is<Password>(v => v.Value == Command.NewPassword))
            .Returns("newPasswordHash");

        passwordHasher.Verify(
                Arg.Is<Password>(v => v.Value == Command.OldPassword),
                user.Value.PasswordHash)
            .Returns(true);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        passwordHasher
            .Received(1)
            .Hash(Arg.Is<Password>(v => v.Value == Command.NewPassword));

        user.Value.PasswordHash.Should().Be("newPasswordHash");
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdateUser()
    {
        // Arrange
        var user = await Factories.User.CreateDefault(
            id: new(Command.UserId),
            password: Password.Create(Command.OldPassword).Value,
            passwordHasher: passwordHasher);

        userRepository.GetAsync(user.Value.Id, Arg.Any<CancellationToken>())
            .Returns(user.Value);

        passwordHasher.Verify(
                Arg.Is<Password>(v => v.Value == Command.OldPassword),
                user.Value.PasswordHash)
            .Returns(true);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await userRepository
            .Received(1)
            .UpdateAsync(user.Value, Arg.Any<CancellationToken>());
    }
}