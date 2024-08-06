using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.Services;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Features.Users;

namespace MyWallet.UnitTests.Features.Users;

[TestSubject(typeof(ChangePasswordHandler))]
public sealed class ChangePasswordHandlerTests
{
    private readonly IUserRepository userRepository = A.Fake<IUserRepository>();
    private readonly IPasswordHasher passwordHasher = A.Fake<IPasswordHasher>();

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

        A.CallTo(() => userRepository.GetAsync(user.Value.Id, A<CancellationToken>._))
            .Returns(user.Value);

        A.CallTo(() => passwordHasher.Verify(
                A<Password>.That.Matches(v => v.Value == Command.OldPassword),
                user.Value.PasswordHash))
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

        A.CallTo(() => userRepository.GetAsync(user.Value.Id, A<CancellationToken>._))
            .Returns(user.Value);

        A.CallTo(() => passwordHasher.Hash(A<Password>.That.Matches(v => v.Value == Command.NewPassword)))
            .Returns("newPasswordHash");

        A.CallTo(() => passwordHasher.Verify(
                A<Password>.That.Matches(v => v.Value == Command.OldPassword),
                user.Value.PasswordHash))
            .Returns(true);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => passwordHasher.Hash(A<Password>.That.Matches(v => v.Value == Command.NewPassword)))
            .MustHaveHappenedOnceExactly();

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

        A.CallTo(() => userRepository.GetAsync(user.Value.Id, A<CancellationToken>._))
            .Returns(user.Value);

        A.CallTo(() => passwordHasher.Verify(
                A<Password>.That.Matches(v => v.Value == Command.OldPassword),
                user.Value.PasswordHash))
            .Returns(true);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => userRepository.UpdateAsync(user.Value, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}