using Microsoft.Extensions.Logging;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Events;
using MyWallet.Features.Users.Shared.Events;

namespace MyWallet.UnitTests.Features.Users.Shared.Events;

[TestSubject(typeof(UserCreatedEventHandler))]
public sealed class UserCreatedEventHandlerTests
{
    private readonly IDefaultCategoriesProvider defaultCategoriesProvider = new DefaultCategoriesProvider();
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly ICategoryRepository categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly ILogger<UserCreatedEvent> logger = Substitute.For<ILogger<UserCreatedEvent>>();

    private readonly UserCreatedEventHandler sut;

    public UserCreatedEventHandlerTests()
    {
        sut = new UserCreatedEventHandler(
            defaultCategoriesProvider,
            userRepository,
            categoryRepository,
            logger);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldCreateDefaultCategoriesForUser()
    {
        // Arrange  
        var user = await Factories.User.CreateDefault();
        var userCreatedEvent = new UserCreatedEvent(user.Value.Id);

        userRepository.GetAsync(user.Value.Id, Arg.Any<CancellationToken>())
            .Returns(user.Value);

        // Act
        await sut.Handle(userCreatedEvent, CancellationToken.None);

        // Assert
        await categoryRepository
            .Received(1)
            .AddRangeAsync(Arg.Any<IEnumerable<Category>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowApplicationException()
    {
        // Arrange
        var userId = UserId.New();
        var userCreatedEvent = new UserCreatedEvent(userId);

        userRepository.GetAsync(userId, Arg.Any<CancellationToken>())
            .Returns(UserErrors.NotFound);

        // Act
        var act = () => sut.Handle(userCreatedEvent, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"User with ID '{userId}' not found.");
    }
}