using Microsoft.Extensions.Logging;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Events;
using MyWallet.Features.Users.Shared.Events;

namespace MyWallet.UnitTests.Features.Users.Shared.Events;

[TestSubject(typeof(UserCreatedEventHandler))]
public sealed class UserCreatedEventHandlerTests
{
    private readonly IDefaultCategoriesProvider defaultCategoriesProvider = new DefaultCategoriesProvider();
    private readonly IUserRepository userRepository = A.Fake<IUserRepository>();
    private readonly ICategoryRepository categoryRepository = A.Fake<ICategoryRepository>();
    private readonly ILogger<UserCreatedEvent> logger = A.Fake<ILogger<UserCreatedEvent>>();

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

        A.CallTo(() => userRepository.GetAsync(user.Value.Id, A<CancellationToken>._))
            .Returns(user.Value);

        // Act
        await sut.Handle(userCreatedEvent, CancellationToken.None);

        // Assert
        A.CallTo(() => categoryRepository.AddRangeAsync(A<IEnumerable<Category>>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowApplicationException()
    {
        // Arrange
        var userId = UserId.New();
        var userCreatedEvent = new UserCreatedEvent(userId);

        A.CallTo(() => userRepository.GetAsync(userId, A<CancellationToken>._))
            .Returns(null as User);

        // Act
        var act = () => sut.Handle(userCreatedEvent, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"User with ID '{userId}' not found.");
    }
}