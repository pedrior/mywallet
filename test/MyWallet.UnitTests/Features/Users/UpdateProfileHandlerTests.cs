using MyWallet.Domain.Users;
using MyWallet.Features.Users;

namespace MyWallet.UnitTests.Features.Users;

[TestSubject(typeof(UpdateProfileHandler))]
public sealed class UpdateProfileHandlerTests
{
    private readonly IUserRepository userRepository = A.Fake<IUserRepository>();

    private readonly UpdateProfileHandler sut;

    private static readonly UpdateProfileCommand Command = new()
    {
        Name = "Jane Doe",
        UserId = Ulid.NewUlid()
    };

    public UpdateProfileHandlerTests()
    {
        sut = new UpdateProfileHandler(userRepository);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        A.CallTo(() => userRepository.GetAsync(
                A<UserId>.That.Matches(v => v.Value == Command.UserId),
                A<CancellationToken>._))
            .Returns(user.Value);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdateUserProfile()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        A.CallTo(() => userRepository.GetAsync(
                A<UserId>.That.Matches(v => v.Value == Command.UserId),
                A<CancellationToken>._))
            .Returns(user.Value);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        user.Value.Name.Value.Should().Be(Command.Name);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdateUser()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        A.CallTo(() => userRepository.GetAsync(
                A<UserId>.That.Matches(v => v.Value == Command.UserId),
                A<CancellationToken>._))
            .Returns(user.Value);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => userRepository.UpdateAsync(user.Value, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}