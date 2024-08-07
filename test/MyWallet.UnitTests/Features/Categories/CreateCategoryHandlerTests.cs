using MyWallet.Domain.Categories;
using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Features.Categories;

namespace MyWallet.UnitTests.Features.Categories;

[TestSubject(typeof(CreateCategoryHandler))]
public sealed class CreateCategoryHandlerTests
{
    private readonly IUserRepository userRepository = A.Fake<IUserRepository>();
    private readonly ICategoryRepository categoryRepository = A.Fake<ICategoryRepository>();

    private readonly CreateCategoryHandler sut;

    private static readonly CreateCategoryCommand Command = new()
    {
        Type = "expense",
        Name = "Shopping",
        Color = "#EF5350",
        UserId = Ulid.NewUlid()
    };

    private static readonly UserId UserId = new(Command.UserId);

    private readonly User user = Factories.User.CreateDefault(id: UserId)
        .Result.Value;

    public CreateCategoryHandlerTests()
    {
        A.CallTo(() => userRepository.GetAsync(UserId, A<CancellationToken>._))
            .Returns(user);

        sut = new CreateCategoryHandler(userRepository, categoryRepository);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldReturnCategoryId()
    {
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<CategoryId>()
            .Which.Value.Should().NotBe(Ulid.Empty);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldPersistCategory()
    {
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => categoryRepository.AddAsync(
                A<Category>.That.Matches(v => v.Id == result.Value),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldAddCategoryToUser()
    {
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        user.CategoryIds.Should().Contain(result.Value);

        A.CallTo(() => userRepository.UpdateAsync(user, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdateUser()
    {
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => userRepository.UpdateAsync(user, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}