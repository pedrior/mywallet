using MyWallet.Domain.Users;
using MyWallet.Features.Users.UpdateProfile;

namespace MyWallet.UnitTests.Features.Users.UpdateProfile;

[TestSubject(typeof(UpdateProfileHandler))]
public sealed class UpdateProfileHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();

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
    public async Task Handle_WhenCommandIsValid_ShouldReturnUpdated()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        userRepository.GetAsync(
                Arg.Is<UserId>(v => v.Value == Command.UserId),
                Arg.Any<CancellationToken>())
            .Returns(user.Value);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Updated);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdateUserProfile()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();

        userRepository.GetAsync(
                Arg.Is<UserId>(v => v.Value == Command.UserId),
                Arg.Any<CancellationToken>())
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

        userRepository.GetAsync(
                Arg.Is<UserId>(v => v.Value == Command.UserId),
                Arg.Any<CancellationToken>())
            .Returns(user.Value);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await userRepository
            .Received(1)
            .UpdateAsync(user.Value, Arg.Any<CancellationToken>());
    }
}