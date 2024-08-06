using MyWallet.Features.Users;
using MyWallet.Shared.Persistence;

namespace MyWallet.UnitTests.Features.Users;

[TestSubject(typeof(ViewProfileHandler))]
public sealed class ViewProfileHandlerTests
{
    private readonly IDbContext dbContext = A.Fake<IDbContext>();

    private readonly ViewProfileHandler sut;

    private static readonly ViewProfileQuery Query = new()
    {
        UserId = Ulid.NewUlid()
    };

    public ViewProfileHandlerTests()
    {
        sut = new ViewProfileHandler(dbContext);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnUserProfileResponseForCurrentUser()
    {
        // Arrange
        var userProfileResponse = new UserProfileResponse
        {
            Name = "John Doe",
            Email = "john@doe.com",
            CreatedAt = DateTimeOffset.UtcNow.AddMonths(-2),
            UpdatedAt = null
        };

        A.CallTo(() => dbContext.QuerySingleOrDefaultAsync<UserProfileResponse>(
                A<string>._,
                A<object>._,
                A<CancellationToken>._))
            .Returns(userProfileResponse);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(userProfileResponse);
    }
}