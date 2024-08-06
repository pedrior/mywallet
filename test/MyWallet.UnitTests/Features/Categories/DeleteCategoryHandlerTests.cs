using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Features.Categories;

namespace MyWallet.UnitTests.Features.Categories;

[TestSubject(typeof(DeleteCategoryHandler))]
public sealed class DeleteCategoryHandlerTests
{
    private readonly IUserRepository userRepository = A.Fake<IUserRepository>();

    private readonly DeleteCategoryHandler sut;

    private static readonly DeleteCategoryCommand Command = new()
    {
        Id = Ulid.NewUlid(),
        UserId = Ulid.NewUlid()
    };

    private static readonly UserId UserId = new(Command.UserId);

    private readonly User user = Factories.User.CreateDefault(id: UserId)
        .Result.Value;

    public DeleteCategoryHandlerTests()
    {
        user.AddCategory(new CategoryId(Command.Id));
        
        A.CallTo(() => userRepository.GetAsync(UserId, A<CancellationToken>._))
            .Returns(user);

        sut = new DeleteCategoryHandler(userRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnDeleted()
    {
        // Arrange
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Deleted>();
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldDeleteCategory()
    {
        // Arrange
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        user.CategoryIds.Should().NotContain(new CategoryId(Command.Id));
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldUpdateUser()
    {
        // Arrange
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => userRepository.UpdateAsync(user, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}